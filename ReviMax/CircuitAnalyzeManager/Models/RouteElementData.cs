using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.CircuitAnalyzeManager.Models
{
    internal class RouteElementData
    {
        public EquipmentNode Element { get; set; } = new();
        public XYZ Start { get; set; } = XYZ.Zero;
        public XYZ End { get; set; } = XYZ.Zero;
        public string Type { get; set; } = string.Empty;
    }
}
