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
    internal class CircuitGraph
    {
        public Dictionary<string, CircuitGraphNode> Nodes { get; set; } = new();
        public Dictionary<string, CircuitGraphEdge> Edges { get; set; } = new();

        public void AddNode(CircuitGraphNode node)
        {
            if (node == null || string.IsNullOrEmpty(node.Id)) return;
            if (!Nodes.ContainsKey(node.Id))
            {
                Nodes[node.Id] = node;
                ReviMaxLog.Information($"Circuit node added: {node.Id}");
            }
        }
        public CircuitGraphNode? GetNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return null;
            Nodes.TryGetValue(nodeId, out var node);
            ReviMaxLog.Information($"Got circuit node: {node.Id}");
            return node;
        }
        public void RemoveNode(string nodeId) 
        {
            if (string.IsNullOrEmpty(nodeId)) return;
            if (Nodes.TryGetValue(nodeId, out var node))
            {
                Nodes.Remove(nodeId);
                ReviMaxLog.Information($"Circuit node deleted: {node.Id}");
            }
        }
        public void AddEdge(CircuitGraphEdge edge)
        {
            if (edge == null || string.IsNullOrEmpty(edge.Id)) return;
            if (!Edges.ContainsKey(edge.Id))
            {
                Edges[edge.Id] = edge;
                // Добавляем ID ребра в соответствующие узлы
                if (Nodes.TryGetValue(edge.StartNodeId, out var startNode))
                {
                    startNode.EdgeIds.Add(edge.Id);
                    ReviMaxLog.Information($"Added edge ID {edge.Id} to start node {startNode.Id}");
                }
                if (Nodes.TryGetValue(edge.EndNodeId, out var endNode))
                {
                    endNode.EdgeIds.Add(edge.Id);
                    ReviMaxLog.Information($"Added edge ID {edge.Id} to end node {endNode.Id}");
                }
                ReviMaxLog.Information($"Circuit edge added: {edge.Id}");
            }
        }
        public CircuitGraphEdge? GetEdge(string edgeId)
            {
                if (string.IsNullOrEmpty(edgeId)) return null;
                Edges.TryGetValue(edgeId, out var edge);
                ReviMaxLog.Information($"Got circuit edge: {edge?.Id}");
                return edge;
        }
        public void RemoveEdge(string edgeId)
        {
            if (string.IsNullOrEmpty(edgeId)) return;
            if (Edges.TryGetValue(edgeId, out var edge))
            {
                // Удаляем ID ребра из соответствующих узлов
                if (Nodes.TryGetValue(edge.StartNodeId, out var startNode))
                {
                    startNode.EdgeIds.Remove(edgeId);
                    ReviMaxLog.Information($"Removed edge ID {edgeId} from start node {startNode.Id}");
                }
                if (Nodes.TryGetValue(edge.EndNodeId, out var endNode))
                {
                    endNode.EdgeIds.Remove(edgeId);
                    ReviMaxLog.Information($"Removed edge ID {edgeId} from end node {endNode.Id}");
                }
                Edges.Remove(edgeId);
            }
            ReviMaxLog.Information($"Circuit edge deleted: {edgeId}");
        }
        }
}
