using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;
using Autodesk.Revit.DB;
using Microsoft.Win32;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Models.Annotations;

namespace ReviMax.GostSymbolManager.Services
{
    internal static class CableSystemSettingsManager 
    {
        public static CableSystemSettings? LoadCableSystemSettingsFromFile(string path)
        {
            if (!File.Exists(path) || Path.GetExtension(path) != ".json")
            {
                ReviMaxLog.Warning($"Cable system settings file not found at {path} or invalid file type.");
                return null;
            }
            JsonConverter.Deserialize<CableSystemSettingsDto>(File.ReadAllText(path), out CableSystemSettingsDto? dto);
            return dto?.ToModel() ?? new();
        }

        public static void  SaveCableSystemSettingsToFile(CableSystemSettings settings, string path, string fileName)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var setingsToSave = settings.ToDto();
            PathManager.CreateDirectoryIfNotExist(path);
            File.WriteAllText(Path.Combine(path, fileName), JsonConverter.Serialize<CableSystemSettingsDto>(setingsToSave));
            ReviMaxLog.Information($"Cable system settings saved to {path}");
        }
    }
}
