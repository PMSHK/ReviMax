using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal static class CircuitGraphManager
    {
        public static CircuitGraph CompressGraph(this CircuitGraph graph)
        {
            bool changed;
            do
            {
                changed = false;
                var nodes = graph.Nodes.Values.ToList();
                foreach (var graphNode in nodes)
                {
                    if (!graph.Nodes.ContainsKey(graphNode.Id)) continue;
                    if (graphNode.IsSpecial) continue;
                    if (graphNode.Degree != 2) continue;


                    var edge1 = graph.Edges.Values.FirstOrDefault(e => e.Id == graphNode.EdgeIds[0]);
                    var edge2 = graph.Edges.Values.FirstOrDefault(e => e.Id == graphNode.EdgeIds[1]);
                    if (edge1 == null || edge2 == null) continue;
                    if (!AreCollinear(graph, graphNode, edge1, edge2)) continue;

                    MergeEdges(graph, graphNode, edge1, edge2);
                    changed = true;
                    break;
                }
                // Логика построения графа на основе routeElements
            }
            while (changed);

            return graph;
        }

        private static void MergeEdges(CircuitGraph graph, CircuitGraphNode middleNode,CircuitGraphEdge edge1, CircuitGraphEdge edge2)
        {
            string firstNodeId =
        edge1.StartNodeId == middleNode.Id
            ? edge1.EndNodeId
            : edge1.StartNodeId;

            string lastNodeId =
                edge2.StartNodeId == middleNode.Id
                    ? edge2.EndNodeId
                    : edge2.StartNodeId;

            if (!graph.Nodes.TryGetValue(firstNodeId, out var nodeA))
                return;

            if (!graph.Nodes.TryGetValue(lastNodeId, out var nodeC))
                return;

            // Создаём merged edge

            var mergedEdge = new CircuitGraphEdge
            {
                Id = Guid.NewGuid().ToString(),

                StartNodeId = nodeA.Id,
                EndNodeId = nodeC.Id,
            };

            // Переносим route elements

            mergedEdge.RunElements.AddRange(edge1.RunElements);
            mergedEdge.RunElements.AddRange(edge2.RunElements);
            mergedEdge.CableIds.AddRange(edge1.CableIds);
            mergedEdge.CableIds.AddRange(edge2.CableIds);
            mergedEdge.Curves.AddRange(edge1.Curves);
            mergedEdge.Curves.AddRange(edge2.Curves);

            // Удаляем старые edges

            graph.RemoveEdge(edge1.Id);
            graph.RemoveEdge(edge2.Id);

            // Добавляем новый edge в graph

            graph.AddEdge(mergedEdge);

            // Удаляем middle node

            graph.RemoveNode(middleNode.Id);
        }

        private static bool TryGetDirection(
            CircuitGraph graph,
            CircuitGraphEdge edge,
            CircuitGraphNode pivot,
            out XYZ direction)
                {
                    direction = XYZ.Zero;

                    if (edge.StartNodeId != pivot.Id && edge.EndNodeId != pivot.Id)
                    {
                        ReviMaxLog.Warning(
                            $"Graph edge skipped because it is not connected to pivot node. " +
                            $"EdgeId={edge.Id}, PivotNodeId={pivot.Id}, StartNodeId={edge.StartNodeId}, EndNodeId={edge.EndNodeId}");
                        return false;
                    }

                    var otherNodeId =
                        edge.StartNodeId == pivot.Id
                            ? edge.EndNodeId
                            : edge.StartNodeId;

                    if (!graph.Nodes.TryGetValue(otherNodeId, out var otherNode))
                    {
                        ReviMaxLog.Warning(
                            $"Graph edge skipped because endpoint node is missing. " +
                            $"EdgeId={edge.Id}, PivotNodeId={pivot.Id}, MissingNodeId={otherNodeId}");
                        return false;
                    }

                    var vector = otherNode.Point - pivot.Point;
                    if (vector.GetLength() < 1e-9)
                    {
                        ReviMaxLog.Warning(
                            $"Graph edge skipped because direction vector is too short. " +
                            $"EdgeId={edge.Id}, PivotNodeId={pivot.Id}, OtherNodeId={otherNode.Id}");
                        return false;
                    }

                    direction = vector.Normalize();
                    return true;
                }
        private static bool AreCollinear(
            CircuitGraph graph,
            CircuitGraphNode node,
            CircuitGraphEdge edge1,
            CircuitGraphEdge edge2,
            double angleToleranceDeg = 3)
                {
                    if (!TryGetDirection(graph, edge1, node, out var dir1) ||
                        !TryGetDirection(graph, edge2, node, out var dir2))
                    {
                        return false;
                    }

                    double angle =
                        dir1.AngleTo(dir2) * 180.0 / Math.PI;

                    return angle < angleToleranceDeg ||
                           Math.Abs(angle - 180) < angleToleranceDeg;
                }
    }
}
