using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using ReviMax.Config;
using ReviMax.Core.Mapper;
using ReviMax.Core.Revit;
using ReviMax.Core.Utils.Config;
using ReviMax.DTO.Annotations;
using ReviMax.Models.Annotations;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Services;
using ReviMax.Services;
using ReviMax.Services.Utils;

namespace ReviMax.Core.Utils.Managers
{
    internal static class ReviMaxSettingsProvider
    {
        public static CableSystemSettings Load(Document doc,RevitDispatcherService dispatcher)
        {
            
            if (ReviMaxProjectSettingsStorage.TryLoad(doc, out var revitSettings))
            {
                var fileName = Path.GetFileNameWithoutExtension(doc.PathName);
                ReviMaxLog.Information($"Loaded settings successfully from {fileName}. Data: {revitSettings?.ToString()}");
                return revitSettings?.ToModel();
            }

            if (SaveLoadFileService.LoadFromFile<CableSystemSettingsDto>(PathManager.GetAppDataPath(),out var fileSettings))
            {
                var fileName = Path.GetFileNameWithoutExtension(fileSettings.path);
                var settings = fileSettings.data.ToModel();
                var familyService = new FamilyService(doc);
                List<string> familyNames = settings.LineSettings.Select(l=>l.Family.Family.FamilyName).Distinct().ToList();

                dispatcher.Request(
                request: (app) => {
                    TransactionManager.StartTransaction(doc, "Update line settings", (d) => ReviMaxProjectSettingsStorage.Save(d, settings.ToDto()));
                });

                familyService.LoadFamilyIfNotLoaded(familyNames);

                ReviMaxLog.Information($"Loaded settings successfully from {fileName}. Data: {settings.ToString()}");
                
                return settings;
            }
            
            var defaultSettings = SettingsService.InitializeStandard();
            ReviMaxLog.Information($"Standard settings Loaded successfully. Data: {defaultSettings.ToString()}");
            return defaultSettings;
            
        }
    }
}
