using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Models;
using ReviMax.Models.Annotations;

namespace ReviMax.DTO.Annotations
{
    internal class CableSystemSettingsDto
    {
        public CableSystemSettingsDto() { }
        public BaseCableSysSettingsDto? GeneralSettings { get; set; }
        public List<ReviLineDto>? LineSettings { get; set; }
        public override string ToString()
        {
            var lineSettingsStr = LineSettings != null ? string.Join(", ", LineSettings.Select(l => l.ToString())) : "null";
            return $"CableSystemSettingsDto: GeneralSettings: {GeneralSettings}, LineSettings: {lineSettingsStr}";
        }
    }
}
