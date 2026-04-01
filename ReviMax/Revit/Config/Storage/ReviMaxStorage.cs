using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Revit.Config.Storage
{
    using System.Collections;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.ExtensibleStorage;
    using ReviMax.Core.Config;

    internal static class ReviMaxStorage
    {
        private static readonly Guid SchemaGuid = new Guid("E806C7BE-B487-4AA7-84BC-2FD81475174A");
        private const string SchemaName = "ReviMax_GOST_Annotation";
        private const string FieldRunId = "RunId";
        private const string FieldType = "Type";
        private const string FieldSourceId = "SourceElementId";
        private const string FieldViewId = "ViewId";

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
            return null;
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
    }
}
