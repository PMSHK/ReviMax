using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Config;
using ReviMax.Core.Mapper;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Managers;
using ReviMax.DTO.Annotations;
using ReviMax.Models;
using ReviMax.Models.Annotations;

namespace ReviMax.Services.Utils
{
    public static class SettingsService
    {
        private static CableSystemSettings _settings;
        public static CableSystemSettings InitializeStandard()
        {
            BaseCableSysSettings baseSettings = new BaseCableSysSettings()
            {
                Step = 500.0 / 304.8,
                Offset = 500 / 304.8,
                Snap = 2.0 / 304.8,
                CornerClearance = 400.0 / 304.8,
                MinDist = 300.0 / 304.8,
                NodeClearance = 250.0 / 304.8
            };

            ReviLine LineSettings = new ReviLine();
            List<ReviLine> Lines = new List<ReviLine>();
            return _settings = new CableSystemSettings(baseSettings, Lines);
        }

        public static CableSystemSettings? LoadFromFile(string path)
        {
            if (!File.Exists(path) || Path.GetExtension(path) != ".json")
            {
                ReviMaxLog.Warning($"Cable system settings file not found at {path} or invalid file type.");
                return null;
            }
            JsonConverter.Deserialize<CableSystemSettingsDto>(File.ReadAllText(path), out CableSystemSettingsDto? dto);
            return dto?.ToModel() ?? new();
        }

        public static void SaveToFile(CableSystemSettings settings, string path, string fileName)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var setingsToSave = settings.ToDto();
            PathManager.CreateDirectoryIfNotExist(path);
            File.WriteAllText(Path.Combine(path, fileName), JsonConverter.Serialize<CableSystemSettingsDto>(setingsToSave));
            ReviMaxLog.Information($"Cable system settings saved to {path}");
        }

    }
}
