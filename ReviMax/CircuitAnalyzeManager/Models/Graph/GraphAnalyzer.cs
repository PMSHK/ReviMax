using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using ReviMax.CircuitAnalyzeManager.Models.Enums;
using ReviMax.Core.Config;
using ReviMax.Core.Utils.Converter;
using ReviMax.Revit.Core.Services;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal class GraphAnalyzer
    {
        /// <summary>
        /// Вычисляет минимальное евклидово расстояние от точки до поверхности или объема BoundingBox.
        /// Если точка внутри BoundingBox, расстояние равно 0.

        private static double GetDistanceFromPointToBoundingBox(XYZ point, BoundingBoxXYZ bbox)
        {
            // Для каждой оси вычисляем, насколько точка выходит за пределы границ.
            // Если точка внутри диапазона [Min, Max] по этой оси, расстояние по этой оси равно 0.

            double dx = Math.Max(0, Math.Max(bbox.Min.X - point.X, point.X - bbox.Max.X));
            double dy = Math.Max(0, Math.Max(bbox.Min.Y - point.Y, point.Y - bbox.Max.Y));
            double dz = Math.Max(0, Math.Max(bbox.Min.Z - point.Z, point.Z - bbox.Max.Z));

            // Теорема Пифагора в 3D для оставшихся "выступов"
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static GraphCandidate? FindGraphCandidate(
    Element element,
    CircuitGraph graph,
    double tolerance)
        {
            var nodeCandidate = FindNodeCandidate(
                element,
                graph,
                tolerance);

            if (nodeCandidate != null)
                return nodeCandidate;

            return FindEdgeCandidate(
                element,
                graph,
                tolerance);
        }

        private static GraphCandidate? FindNodeCandidate(
    Element element,
    CircuitGraph graph,
    double tolerance)
        {
            var bbox = element.get_BoundingBox(null);

            if (bbox == null)
                return null;

            foreach (var node in graph.Nodes.Values)
            {
                double distance =
                    GetDistanceFromPointToBoundingBox(
                        node.Point,
                        bbox);

                if (distance <= tolerance)
                {
                    return new GraphCandidate
                    {
                        ElementId = element.Id,
                        LocationType = CandidateLocation.Node,
                        NodeId = node.Id,
                        ProjectionPoint = node.Point,
                        Distance = distance
                    };
                }
            }

            return null;
        }

        private static GraphCandidate? FindEdgeCandidate(
    Element element,
    CircuitGraph graph,
    double tolerance)
        {
            var bbox = element.get_BoundingBox(null);

            if (bbox == null)
                return null;

            XYZ center = (bbox.Min + bbox.Max) / 2.0;

            GraphCandidate? best = null;

            foreach (var edge in graph.Edges.Values)
            {
                if (!graph.Nodes.TryGetValue(edge.StartNodeId, out var startNode) ||
                    !graph.Nodes.TryGetValue(edge.EndNodeId, out var endNode))
                {
                    ReviMaxLog.Warning(
                        $"Graph edge skipped because endpoint node is missing. " +
                        $"EdgeId={edge.Id}, StartNodeId={edge.StartNodeId}, EndNodeId={edge.EndNodeId}");
                    continue;
                }

                XYZ projectedPoint;
                double t;

                double distance =
                    DistanceToSegment(
                        center,
                        startNode.Point,
                        endNode.Point,
                        out projectedPoint,
                        out t);


                Element? hostElement = RevitElementsManager.GetElementById(element.Document, edge.RunElements.Last().Id);
                
                var maxTrayHalfWidth = edge.RunElements
                    .Select(r =>
                    {
                        Element? e = RevitElementsManager.GetElementById(element.Document, r.Id);

                        if (e is CableTray tray)
                            return tray.Width/2;

                        if (e is Conduit conduit)
                            return conduit.Diameter/2;

                        return 0;
                    })
                    .Max();

                if (distance > tolerance + maxTrayHalfWidth)
                    continue;

                if (t < 0.05 || t > 0.95)
                    continue;

                if (best == null || distance < best.Distance)
                {
                    best = new GraphCandidate
                    {
                        ElementId = element.Id,
                        LocationType = CandidateLocation.Edge,
                        EdgeId = edge.Id,
                        ProjectionPoint = projectedPoint,
                        Distance = distance
                    };
                }
            }

            return best;
        }

        private static double DistanceToSegment(
    XYZ point,
    XYZ start,
    XYZ end,
    out XYZ projectedPoint,
    out double t)
        {
            XYZ segment = end - start;

            double len2 = segment.DotProduct(segment);

            if (len2 < 1e-9)
            {
                projectedPoint = start;
                t = 0;
                return point.DistanceTo(start);
            }

            t = (point - start).DotProduct(segment) / len2;

            t = Math.Max(0, Math.Min(1, t));

            projectedPoint = start + segment.Multiply(t);

            return point.DistanceTo(projectedPoint);
        }

    }
}
