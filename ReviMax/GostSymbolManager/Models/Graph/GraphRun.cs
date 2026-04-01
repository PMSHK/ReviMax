using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Models.Revit;

namespace ReviMax.GostSymbolManager.Models.Graph
{
    internal class GraphRun
    {
        public GraphNode StartNode { get; set; } = null!;
        public GraphNode EndNode { get; set; } = null!;

        public GraphEdge FirstEdge { get; set; } = null!;
        public GraphEdge LastEdge { get; set; } = null!;

        public List<AxisSegment> Segments { get; set; } = [];
    }
}
