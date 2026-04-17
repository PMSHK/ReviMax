using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.CircuitAnalyzeManager.Models
{
    internal class ElectricalPathData
    {
        public string CircuitId { get; set; } = string.Empty;
        public List<XYZ> PathPoints { get; set; } = new List<XYZ>();
        public ElementId PanelId { get; set; } = ElementId.InvalidElementId;
        public List<ElementId> ConnectedEquipmentIds { get; set; } = new List<ElementId>();
    }
}
