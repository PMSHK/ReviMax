using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.CircuitAnalyzeManager.Models.Enums;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal class CircuitGraphNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public CircuitGraphNodeType Type { get; set; }
        public ElementId? ElementId { get; set; }
        public XYZ Point { get; set; } = XYZ.Zero;
        public List<string> EdgeIds { get; set; } = new();
        public int Degree => EdgeIds.Count;
        public bool IsSpecial => Type == CircuitGraphNodeType.JunctionBox || Type == CircuitGraphNodeType.Equipment;

    }
}
