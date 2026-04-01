using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.Models.Graph.Filter
{
    internal static class NodeFilter
    {
        public static bool IsCornerNode(GraphNode node, double angleDegThreshold = 10.0)
        {
            if (node.Edges.Count != 2) return false;

            var e1 = node.Edges[0];
            var e2 = node.Edges[1];

            XYZ d1 = GetOutgoingDir(node, e1);
            XYZ d2 = GetOutgoingDir(node, e2);

            // угол между направлениями
            double ang = d1.AngleTo(d2) * 180.0 / Math.PI;

            // если не почти 180°, значит есть поворот
            return Math.Abs(180.0 - ang) > angleDegThreshold;
        }

        public static XYZ GetOutgoingDir(GraphNode node, GraphEdge edge)
        {
            // направление "из узла" к другому концу ребра
            XYZ other =
                (edge.From == node) ? edge.To.Point :
                (edge.To == node) ? edge.From.Point :
                // fallback на Segment
                edge.Segment.EndPoint;

            XYZ v = other - node.Point;
            return v.IsZeroLength() ? XYZ.BasisX : v.Normalize();
        }
    }
}
