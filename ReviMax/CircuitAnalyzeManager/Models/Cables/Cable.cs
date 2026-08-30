using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.CircuitAnalyzeManager.Models.Cables
{
    internal class Cable
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public double Length { get; set; } = 0;
        public double CrossSection { get; set; } = 0;
        public string GostNumber { get; set; } = string.Empty;
        public string CableCode { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string MeasureUnit { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }
}
