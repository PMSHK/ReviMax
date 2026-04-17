using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Exceptions;
using ReviMax.Handlers;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal class CircuitGraphBuilder
    {
        public CircuitGraph Graph { get; set; } = new CircuitGraph();
        
        public ResultHandler<CircuitGraph> BuildGraph(IEnumerable<RouteElementData> cableSystems, double tolerance) 
        {
            if (cableSystems == null) return ResultHandler<CircuitGraph>.Failure("No cable systems provided.");
            foreach (var cableSystem in cableSystems)
            {
                var startNode = GetOrCreateNode(cableSystem.Start, tolerance);
                var endNode = GetOrCreateNode(cableSystem.End, tolerance);
                var edge = GetOrCreateEdge(startNode, endNode, cableSystem.Element);
            }

            return ResultHandler<CircuitGraph>.Success(Graph);
        }

        private CircuitGraphNode GetOrCreateNode(XYZ point, double tolerance)
        {
            foreach (var node in Graph.Nodes)
            {
                if (node.Value?.Point?.DistanceTo(point) <= tolerance)
                {
                    return node.Value;
                }
            }
            var newNode = new CircuitGraphNode
            {
                Id = Guid.NewGuid().ToString(),
                Point = point,
                EdgeIds = new List<string>()
            };

            Graph.AddNode(newNode);
            return newNode;
        }

        private CircuitGraphEdge GetOrCreateEdge(CircuitGraphNode startNode, CircuitGraphNode endNode, EquipmentNode hostElement)
        {
            foreach (var edge in Graph.Edges)
            {
                if ((edge.Value.StartNodeId == startNode.Id && edge.Value.EndNodeId == endNode.Id) ||
                    (edge.Value.StartNodeId == endNode.Id && edge.Value.EndNodeId == startNode.Id))
                {
                    return edge.Value;
                }
            }
            var newEdge = new CircuitGraphEdge
            {
                Id = Guid.NewGuid().ToString(),
                StartNodeId = startNode.Id,
                EndNodeId = endNode.Id,
                HostElement = hostElement,
                Length = startNode.Point.DistanceTo(endNode.Point),

            };
            startNode.EdgeIds.Add(newEdge.Id);
            endNode.EdgeIds.Add(newEdge.Id);
            Graph.AddEdge(newEdge);
            return newEdge;
        }
    }
}
