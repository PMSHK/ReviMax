using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.GostSymbolManager.Models.Revit;

namespace ReviMax.GostSymbolManager.Models.Graph
{
    internal class GraphEdge
    {
        public GraphNode From { get; set; }
        public GraphNode To { get; set; }
        public AxisSegment Segment { get; set; }

    }
}
