using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Models.Revit;

namespace ReviMax.Models.Graph
{
    internal class GraphEdge
    {
        public GraphNode From { get; set; }
        public GraphNode To { get; set; }
        public AxisSegment Segment { get; set; }

    }
}
