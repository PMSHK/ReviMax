using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.CircuitAnalyzeManager.Models.Cables;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal class CircuitGraphEdge
    {
        public string Id { get; set; } = string.Empty;
        public string StartNodeId { get; set; } = string.Empty;
        public string EndNodeId { get; set; } = string.Empty;
        public List<string> CableIds { get; set; } = [];
        public List<RouteElementInfo> RunElements { get; set; } = new();
        public List<Curve> Curves { get; set; } = new();
        public double Length => RunElements.Sum(e => e.Length);
    }
}
