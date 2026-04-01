using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.GostSymbolManager.Models.Graph
{
    internal class GraphNode
    {
        public Element Element { get; set; } = null!;
        public XYZ Point { get; set; } = XYZ.Zero;
        public List<GraphEdge> Edges { get; set; } = [];
    }
}
