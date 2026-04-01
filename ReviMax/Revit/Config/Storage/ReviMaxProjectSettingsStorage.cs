using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Newtonsoft.Json;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Models.Annotations;

namespace ReviMax.Revit.Config.Storage
{
    internal class ReviMaxProjectSettingsStorage
    {
        private static readonly Guid SchemaGuid = new Guid("4900586D-1F8F-4A90-B0FF-196E13C07E3A");
        private const string SchemaName = "ReviMax_Settings";
        private const string FieldVersion = "Version";
        private const string FieldSettingsJson = "SettingsJson";

        private static Schema GetOrCreateSchema()
        {
            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema != null)
                return schema;

            var builder = new SchemaBuilder(SchemaGuid);
            builder.SetSchemaName(SchemaName);
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);

            builder.AddSimpleField(FieldVersion, typeof(int));
            builder.AddSimpleField(FieldSettingsJson, typeof(string));

            return builder.Finish();
        }

        public static void Save(Document doc, CableSystemSettingsDto settings)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            Schema schema = GetOrCreateSchema();

            try
            {
                string json = JsonConvert.SerializeObject(settings);

                Entity entity = new Entity(schema);
                entity.Set(schema.GetField(FieldVersion), 1);
                entity.Set(schema.GetField(FieldSettingsJson), json);

                doc.ProjectInformation.SetEntity(entity);
                ReviMaxLog.Information($"Cable system settings saved to project information. Data: {settings.ToString()}");
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Failed to save settings to ExtensibleStorage: {ex}");
            }
        }

        public static bool TryLoad(Document doc, out CableSystemSettingsDto? settings)
        {
            settings = null;
            if (doc == null) return false;

            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema == null) return false;

            Entity entity = doc.ProjectInformation.GetEntity(schema);
            if (!entity.IsValid()) return false;
            string json = entity.Get<string>(schema.GetField(FieldSettingsJson));
            if (string.IsNullOrWhiteSpace(json)) return false;

            try
            {
                int version = entity.Get<int>(schema.GetField(FieldVersion));
                if (version != 1)
                {
                    ReviMaxLog.Warning($"Unsupported settings version: {version}");
                    return false;
                }


                settings = JsonConvert.DeserializeObject<CableSystemSettingsDto>(json);
                return settings != null;
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Failed to load settings from ExtensibleStorage: {ex}");
                settings = null;
                return false;
            }
        }
    }
}
