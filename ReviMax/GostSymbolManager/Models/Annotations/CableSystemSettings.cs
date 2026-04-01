using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.GostSymbolManager.Models;

namespace ReviMax.GostSymbolManager.Models.Annotations
{
    public class CableSystemSettings 
    {
        private BaseCableSysSettings _baseSettings;
        private List<ReviLine> _lineSettings;
        public BaseCableSysSettings GeneralSettings { get => _baseSettings; set => _baseSettings = value; }
        public List<ReviLine> LineSettings { get => _lineSettings; set => _lineSettings = value; }
        public CableSystemSettings()
        {
            _baseSettings = new BaseCableSysSettings();
            _lineSettings = [];
        }
        public CableSystemSettings(BaseCableSysSettings generalSettings, List<ReviLine> lineSettings)
        {
            _baseSettings = generalSettings ?? new BaseCableSysSettings();
            _lineSettings = lineSettings ?? [];
        }

        public void AddLineSetting(ReviLine line)
        {
            if (line != null)
            {
                _lineSettings.Add(line);
            }
        }

        public void RemoveLineSetting(ReviLine line)
        {
            if (line != null)
            {
                _lineSettings.Remove(line);
            }
        }

        public override string ToString()
        {
            var _lineSettingsStr = string.Join(", ", LineSettings.Select(l => l.ToString()));
            return $"CableSystemSettings: GeneralSettings: {GeneralSettings}, LineSettings: {_lineSettingsStr}";
        }

        public override bool Equals(object? obj)
        {
            return obj is CableSystemSettings settings &&
                   EqualityComparer<BaseCableSysSettings>.Default.Equals(_baseSettings, settings._baseSettings) &&
                   _lineSettings.SequenceEqual(settings._lineSettings) &&
                   EqualityComparer<BaseCableSysSettings>.Default.Equals(GeneralSettings, settings.GeneralSettings);
        }

        public override int GetHashCode()
        {
            int hashCode = 1171660647;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseCableSysSettings>.Default.GetHashCode(_baseSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ReviLine>>.Default.GetHashCode(_lineSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseCableSysSettings>.Default.GetHashCode(GeneralSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ReviLine>>.Default.GetHashCode(LineSettings);
            return hashCode;
        }

        public bool Filled()
        {
            return _baseSettings != null && _lineSettings != null && _lineSettings.Count > 0 &&
                _baseSettings.Filled() && _lineSettings.All(l => l.Filled());
        }
        public void CoppyFrom(CableSystemSettings source)
        {
            if (ReferenceEquals(this, source)) return;
            if (source == null) return;
            GeneralSettings.CoppyFrom(source.GeneralSettings);
            LineSettings.Clear();
            foreach (var line in source.LineSettings)
            {
                var newLine = new ReviLine();
                newLine.CoppyFrom(line);
                LineSettings.Add(newLine);
            }
        }
    }
}
