using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Models.Revit;

namespace ReviMax.GostSymbolManager.Models.Graph
{
    internal class GraphBuilder
    {
        private readonly double Tolerance;
        public List<GraphNode> Nodes { get; set; } = new();
        public GraphBuilder(double tolerance)
        {
            Tolerance = tolerance;
        }
        public List<GraphNode> Build(IEnumerable<AxisSegment> segments)
        {
            foreach (var segment in segments)
            {
                var node1 = GetOrCreateNode(segment.StartPoint, segment.Element);
                var node2 = GetOrCreateNode(segment.EndPoint, segment.Element);

                var edge1 = new GraphEdge
                {
                    From = node1,
                    To = node2,
                    Segment = segment
                };

                var edge2 = new GraphEdge
                {
                    From = node2,
                    To = node1,
                    Segment = segment
                };

                node1.Edges.Add(edge1);
                node2.Edges.Add(edge2);
            }
            return Nodes;
        }



        private GraphNode GetOrCreateNode(XYZ point, Element element)
        {
            var existingNode = Nodes.FirstOrDefault(n => n.Point.AreEqual(point,Tolerance));
            if (existingNode != null) return existingNode;
            var node = new GraphNode { Point = point, Element = element };
            Nodes.Add(node);
            return node;
        }
    }
}
