using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal class CircuitGraphNode
    {
        public string Id { get; set; } = string.Empty;
        public EquipmentNode HostElement { get; set; } = new();
        //public ElementId? HostElementId { get; set; } // ID элемента узла
        //public string HostUniqueId { get; set; } = string.Empty; // UniqueID элемента узла
        public XYZ Point { get; set; } = XYZ.Zero;
        public List<string> EdgeIds { get; set; } = new();
        
    }
}
