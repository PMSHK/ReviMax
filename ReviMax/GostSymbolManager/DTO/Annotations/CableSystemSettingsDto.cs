using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.GostSymbolManager.Models;
using ReviMax.GostSymbolManager.Models.Annotations;

namespace ReviMax.GostSymbolManager.DTO.Annotations
{
    internal class CableSystemSettingsDto
    {
        public CableSystemSettingsDto() { }
        public BaseCableSysSettingsDto? GeneralSettings { get; set; }
        public Dictionary<string,List<ReviLineDto>>? DocLineSettings { get; set; }
        public override string ToString()
        {
            var lineSettingsStr = DocLineSettings != null ? 
                string.Join(", ", DocLineSettings.Select(kvp=>$"{kvp.Key}: " +
                $"{string.Join(", ", kvp.Value.Select(line=> line.ToString()))}")) : "null";
            return $"CableSystemSettingsDto: GeneralSettings: {GeneralSettings}, DocLineSettings: {lineSettingsStr}";
        }
    }
}
