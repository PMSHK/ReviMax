using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Revit.Config.Storage
{
    using System.Collections;
    using System.Windows.Documents;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.ExtensibleStorage;
    using ReviMax.Core.Config;
    using ReviMax.Revit.Config.Storage.Model;
    using ReviMax.Revit.Core.Services;

    internal static class ReviMaxStorage
    {
        private static readonly Guid SchemaGuid = new Guid("E806C7BE-B487-4AA7-84BC-2FD81475174A");
        private const string SchemaName = "ReviMax_GOST_Annotation";
        private const string FieldRunId = "RunId";
        private const string FieldType = "Type";
        private const string FieldSourceId = "SourceElementId";
        private const string FieldViewId = "ViewId";
        private const string FieldSourceIds = "SourceElementsIds";

        private static Schema GetOrCreateSchema()
        {
            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema != null) return schema;

            var builder = new SchemaBuilder(SchemaGuid);
            builder.SetSchemaName(SchemaName);
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);

            builder.AddSimpleField(FieldRunId, typeof(string));
            builder.AddSimpleField(FieldType, typeof(string));
            builder.AddSimpleField(FieldSourceId, typeof(int));
            builder.AddSimpleField(FieldViewId, typeof(int));
            builder.AddArrayField(FieldSourceIds, typeof(int));
            return builder.Finish();
        }

        public static void Stamp(Element e, string runId, string type, ElementId sourceId, ElementId viewId)
        {
            Schema schema = GetOrCreateSchema();
            var entity = new Entity(schema);
            entity.Set(schema.GetField(FieldRunId), runId);
            entity.Set(schema.GetField(FieldType), type);
            entity.Set(schema.GetField(FieldSourceId), sourceId.IntegerValue);
            entity.Set(schema.GetField(FieldViewId), viewId.IntegerValue);

            e.SetEntity(entity);
        }

        public static void Stamp(Element e, string runId, string type, List<ElementId> sourceIds, ElementId viewId)
        {
            List<int> ids = sourceIds
                .Where(id => id != null && id != ElementId.InvalidElementId)
                .Select(id => id.IntegerValue)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                throw new ArgumentException("sourceIds does not contain valid ids", nameof(sourceIds));

            Schema schema = GetOrCreateSchema();
            var entity = new Entity(schema);
            entity.Set(schema.GetField(FieldRunId), runId);
            entity.Set(schema.GetField(FieldType), type);
            entity.Set(schema.GetField(FieldSourceId), ids[0]);
            entity.Set(schema.GetField(FieldViewId), viewId.IntegerValue);
            entity.Set<IList<int>>(schema.GetField(FieldSourceIds), ids);

            e.SetEntity(entity);
        }

        public static bool TryGetRunId(Element e, out string runId)
        {
            runId = null;
            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema == null) return false;

            Entity ent = e.GetEntity(schema);
            if (!ent.IsValid()) return false;

            runId = ent.Get<string>(schema.GetField(FieldRunId));
            return !string.IsNullOrEmpty(runId);
        }

        public static bool HasRunId(Element e, string runId)
        {
            if (!TryGetRunId(e, out var r)) return false;
            return string.Equals(r, runId, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetRunId(Element e)
        {
            if (TryGetRunId(e, out var runId))
            {
                ReviMaxLog.Information($"Found element {e}, it has runId: {runId}");
                return runId;
            }
            ReviMaxLog.Information($"Element {e} has no runId.");
            return string.Empty;
        }

        public static ICollection<ElementId> GetElementIdsByRunId(
            Document doc,
            string runId,
            ElementId viewId = null,
            ICollection<BuiltInCategory> categories = null)
        {
            if(doc == null) return [];
            if (string.IsNullOrEmpty(runId)) return [];

            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema == null) return [];

            Field runIdField = schema.GetField(FieldRunId);

            FilteredElementCollector collector = (viewId!=null) ? 
                new FilteredElementCollector(doc, viewId) :
                new FilteredElementCollector(doc);

            collector.WhereElementIsNotElementType();

            if (categories != null && categories.Count > 0)
            {
                var catFilters = categories
            .Select(c => (ElementFilter)new ElementCategoryFilter(c))
            .ToList();

                collector.WherePasses(new LogicalOrFilter(catFilters));
            }

            collector.WherePasses(new ExtensibleStorageFilter(schema.GUID));

            List<ElementId> result = new();

            foreach (Element e in collector)
            {
                Entity ent = e.GetEntity(schema);
                if (!ent.IsValid()) continue;

                string r = ent.Get<string>(runIdField);
                if (string.IsNullOrEmpty(r)) continue;

                if (string.Equals(r, runId, StringComparison.OrdinalIgnoreCase))
                    result.Add(e.Id);
            }

            return result;

        }

        public static Dictionary<string,List<StoredInstanceInfo>> GetInstanceByActiveView(
            View activeView)
        {
            if (activeView == null) return [];
            Document doc = activeView.Document;
            ElementId viewId = activeView.Id;

            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema == null) return [];

            FilteredElementCollector collector = new FilteredElementCollector(doc, viewId)
                .WherePasses(new ExtensibleStorageFilter(schema.GUID));

            List<StoredInstanceInfo> result = new();

            Field runIdField = schema.GetField(FieldRunId);
            Field viewIdField = schema.GetField(FieldViewId);
            Field sourceIdsField = schema.GetField(FieldSourceIds);

            foreach (Element e in collector)
            {
                Entity ent = e.GetEntity(schema);
                if (!ent.IsValid()) continue;

                int storedViewId = ent.Get<int>(viewIdField);
                if (storedViewId != viewId.IntegerValue) continue;

                var rawIds = ent.Get<IList<int>>(sourceIdsField) ?? new List<int>();
                var runId = ent.Get<string>(runIdField);

                result.Add(new StoredInstanceInfo
                {
                    InstanceId = e.Id,
                    RunId = ent.Get<string>(runIdField),
                    ViewId = storedViewId,
                    SourceIds = rawIds.Select(i => new ElementId(i)).ToList()
                });
            }

            return result.GroupBy(s=>s.RunId).ToDictionary(g=>g.Key, g=>g.ToList());
        }

        public static List<ElementId> GetInstanceByRunidActiveView(string runId, View activeView )
        {
            List<ElementId> result = new();
            var instances = GetInstanceByActiveView(activeView);
            return instances.Where(i => string.Equals(i.Key, runId, StringComparison.OrdinalIgnoreCase))
                .SelectMany(m => m.Value).SelectMany(l=> l.SourceIds).Distinct().ToList();

        }

    }
}
