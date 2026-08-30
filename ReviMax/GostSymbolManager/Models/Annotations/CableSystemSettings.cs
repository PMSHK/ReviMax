using Autodesk.Revit.DB;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.Models;
using ReviMax.Revit.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReviMax.GostSymbolManager.Models.Annotations
{
    public class CableSystemSettings 
    {
        private BaseCableSysSettings _baseSettings;
        private Dictionary<string, List<ReviLine>> _docLineSettings = [];
        public BaseCableSysSettings GeneralSettings { get => _baseSettings; set => _baseSettings = value; }
        public Dictionary<string,List<ReviLine>> DocLineSettings { get => _docLineSettings; set => _docLineSettings = value; }
        public CableSystemSettings()
        {
            _baseSettings = new BaseCableSysSettings();
            _docLineSettings = [];
        }
        public CableSystemSettings(BaseCableSysSettings generalSettings, Document doc, List<ReviLine> lineSettings)
        {
            _baseSettings = generalSettings ?? new BaseCableSysSettings();
            _docLineSettings[GetDocumentGuidV5(doc)] = lineSettings ?? [];
        }

        public void AddLineSetting(Document doc, ReviLine line)
        {
            var docGuid = GetDocumentGuidV5(doc);
             if (line == null || string.IsNullOrEmpty(docGuid)) return;
                if (!_docLineSettings.TryGetValue(docGuid, out var lines))
                {
                    lines = new List<ReviLine>();
                    _docLineSettings[docGuid] = lines;
                    
                }
            if (lines.Any(l => l.Equals(line))) return;

            lines.Add(line);
        }

        public void RemoveLineSetting(Document doc,ReviLine line)
        {
            if (line != null)
            {
                _docLineSettings.TryGetValue(GetDocumentGuidV5(doc), out var lines);
                if (lines != null)
                {
                    lines.RemoveAll(l => l.Equals(line));
                }
            }
        }

        public override string ToString()
        {
            
            var _lineSettingsStr = string.Join(", ", DocLineSettings.Select(l => l.Value.Select(val => val.ToString())));
            return $"CableSystemSettings: GeneralSettings: {GeneralSettings}, LineSettings: {_lineSettingsStr}";
        }

        public bool Filled()
        {
            return _baseSettings != null && _docLineSettings != null && _docLineSettings.Count > 0 &&
                _baseSettings.Filled() && _docLineSettings.All(l => l.Value.All(val=>val.Filled()));
        }
        public void CopyFrom(CableSystemSettings source, Document openedDoc)
        {
            if (ReferenceEquals(this, source)) return;
            if (source == null) return;
            GeneralSettings.CoppyFrom(source.GeneralSettings);

            var keysToRemove = DocLineSettings.Keys
            .Where(k => !source.DocLineSettings.ContainsKey(k) && k != GetDocumentGuidV5(openedDoc))
            .ToList();
            foreach (var key in keysToRemove)
            {
                DocLineSettings.Remove(key);
            }

            foreach (var kvp in source.DocLineSettings)
            {
                var sourceKey = kvp.Key;          // GUID документа из файла
                var sourceLines = kvp.Value;     // Его линии

                if (DocLineSettings.TryGetValue(sourceKey, out var existingLines))
                {
                    // Сценарий А: такой документ уже есть в текущем проекте
                    foreach (var parsedLine in sourceLines)
                    {
                        var existingLine = existingLines.FirstOrDefault(l => l.Name == parsedLine.Name);
                        if (existingLine != null)
                        {
                            existingLine.CopyFrom(parsedLine);
                        }
                        else
                        {
                            var newLine = new ReviLine();
                            newLine.CopyFrom(parsedLine);
                            existingLines.Add(newLine);
                        }
                    }
                    existingLines.RemoveAll(l => !sourceLines.Any(pl => pl.Name == l.Name));
                }
                else
                {
                    // Сценарий Б: такого документа нет в текущем проекте (новая связь, новый главный)
                    var newLines = new List<ReviLine>();
                    foreach (var parsedLine in sourceLines)
                    {
                        var newLine = new ReviLine();
                        newLine.CopyFrom(parsedLine);
                        newLines.Add(newLine);
                    }
                    DocLineSettings[sourceKey] = newLines;
                }
            }

        }

        public override bool Equals(object? obj)
        {
            return obj is CableSystemSettings settings &&
                   EqualityComparer<BaseCableSysSettings>.Default.Equals(_baseSettings, settings._baseSettings) &&
                   EqualityComparer<Dictionary<string, List<ReviLine>>>.Default.Equals(_docLineSettings, settings._docLineSettings) &&
                   EqualityComparer<BaseCableSysSettings>.Default.Equals(GeneralSettings, settings.GeneralSettings) &&
                   EqualityComparer<Dictionary<string, List<ReviLine>>>.Default.Equals(DocLineSettings, settings.DocLineSettings);
        }

        public override int GetHashCode()
        {
            int hashCode = 1793646381;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseCableSysSettings>.Default.GetHashCode(_baseSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, List<ReviLine>>>.Default.GetHashCode(_docLineSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseCableSysSettings>.Default.GetHashCode(GeneralSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, List<ReviLine>>>.Default.GetHashCode(DocLineSettings);
            return hashCode;
        }
        private string GetDocumentGuidV5(Document doc)
        {
            var docIdentificator = ProjectInfoManager.GetDocumentIdentificationString(doc);
            return GuidBuilder.CreateVersion5Guid(docIdentificator).ToString();

        }
    }

}
