using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Models.Revit;

namespace ReviMax.GostSymbolManager.Models.Graph
{
    internal class GraphRunsExtractor
    {
        //const double angleToleranceDeg = 3.0;
        public List<GraphRun> ExtractRuns(List<GraphNode> nodes, View view, double angleToleranceDeg = 3.0)
        {
            var runs = new List<GraphRun>();
            var visited = new HashSet<AxisSegment>();

            foreach (var node in nodes)
            {
                if (!IsBreakNode(node, view, angleToleranceDeg))
                    continue;

                foreach (var edge in node.Edges)
                {
                    if (visited.Contains(edge.Segment))
                        continue;

                    var run = BuildRun(node, edge, visited, view, angleToleranceDeg);
                    if (run != null && run.Segments.Count > 0)
                        runs.Add(run);
                }
            }

            // На случай замкнутых контуров без "разрывных" узлов
            foreach (var node in nodes)
            {
                foreach (var edge in node.Edges)
                {
                    if (visited.Contains(edge.Segment))
                        continue;

                    var run = BuildRun(node, edge, visited, view, angleToleranceDeg);
                    if (run != null && run.Segments.Count > 0)
                        runs.Add(run);
                }
            }

            return runs;
        }

        private GraphRun? BuildRun(
    GraphNode startNode,
    GraphEdge firstEdge,
    HashSet<AxisSegment> visited,
    View view,
    double angleToleranceDeg)
        {
            var run = new GraphRun();
            run.StartNode = startNode;
            run.FirstEdge = firstEdge;

            GraphNode currentNode = startNode;
            GraphEdge currentEdge = firstEdge;

            while (true)
            {
                if (visited.Contains(currentEdge.Segment))
                    break;

                visited.Add(currentEdge.Segment);
                run.Segments.Add(currentEdge.Segment);

                GraphNode nextNode = GetOtherNode(currentEdge, currentNode);

                if (IsBreakNode(nextNode, view, angleToleranceDeg))
                {
                    run.EndNode = nextNode;
                    run.LastEdge = currentEdge;
                    break;
                }

                // у обычного промежуточного узла должно быть 2 ребра
                var nextEdge = nextNode.Edges.FirstOrDefault(e => !ReferenceEquals(e.Segment, currentEdge.Segment));
                if (nextEdge == null)
                {
                    run.EndNode = nextNode;
                    run.LastEdge = currentEdge;
                    break;
                }

                currentNode = nextNode;
                currentEdge = nextEdge;
            }

            run.EndNode ??= currentNode;
            run.LastEdge??= currentEdge;
            return run;
        }


        private bool IsBreakNode(GraphNode node, View view, double angleToleranceDeg = 3.0)
        {
            if (node.Edges.Count != 2)
                return true;

            var e1 = node.Edges[0];
            var e2 = node.Edges[1];

            XYZ d1 = GetEdgeDirectionFromNode(node, e1, view);
            XYZ d2 = GetEdgeDirectionFromNode(node, e2, view);

            return !IsCollinear(d1, d2, angleToleranceDeg);
        }


        private XYZ GetEdgeDirectionFromNode(GraphNode node, GraphEdge edge, View view)
        {
            var other = GetOtherNode(edge, node);
            XYZ dir = (other.Point - node.Point).Normalize();
            return ProjectDirectionToViewPlane(dir, view);
        }

        private GraphNode GetOtherNode(GraphEdge edge, GraphNode node)
        {
            return ReferenceEquals(edge.From, node) ? edge.To : edge.From;
        }

        private XYZ ProjectDirectionToViewPlane(XYZ dir, View view)
        {
            XYZ n = view.ViewDirection.Normalize();
            XYZ projected = dir - n.Multiply(dir.DotProduct(n));

            if (projected.GetLength() < 1e-9)
                return XYZ.Zero;

            return projected.Normalize();
        }

        private bool IsCollinear(XYZ d1, XYZ d2, double angleToleranceDeg = 3.0)
        {
            if (d1.IsAlmostEqualTo(XYZ.Zero) || d2.IsAlmostEqualTo(XYZ.Zero))
                return false;

            double cosTol = Math.Cos(angleToleranceDeg * Math.PI / 180.0);
            double dot = Math.Abs(d1.Normalize().DotProduct(d2.Normalize()));
            return dot >= cosTol;
        }








        

    }
}
