using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.Core.Utils.Config;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Services.Utils;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Config.Storage;

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
