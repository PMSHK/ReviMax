using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Models.Revit;
using ReviMax.GostSymbolManager.Models.Graph.Filter;

namespace ReviMax.GostSymbolManager.Models.Graph
{
    internal class PathWalker
    {
        //HashSet<GraphEdge> visited;
        private readonly HashSet<AxisSegment> _visitedSegments;

        public PathWalker()
        {
            //visited = new HashSet<GraphEdge>();
            _visitedSegments = new HashSet<AxisSegment>();
        }

        public PathWalker(HashSet<AxisSegment> visitedSegments)
        {
            //visited = visitedEdges;
            _visitedSegments = visitedSegments ?? new HashSet<AxisSegment>();
        }

        public IEnumerable<(XYZ point, Curve curve, double param, XYZ tangentDir)> Walk(
       GraphNode start,
       double step)
        {
            foreach (var edge in start.Edges)
            {
                foreach (var (p, c, u, tan, _) in WalkEdge(edge, start, null, step, carryIn: 0.0))
                    yield return (p, c, u, tan);
            }
        }

        IEnumerable<(XYZ point, Curve curve, double param, XYZ tangentDir, double carryOut)> WalkEdge(
            GraphEdge edge,
            GraphNode from,
            GraphNode? cameFrom,
            double step,
            double carryIn)
        {
            if (_visitedSegments.Contains(edge.Segment))
                yield break;

            _visitedSegments.Add(edge.Segment);

            var curve = edge.Segment.Axis;
            double len = curve.Length;

            GraphNode nextNode = edge.From == from ? edge.To : edge.From;
            XYZ travelDir = (nextNode.Point - from.Point).Normalize();

            XYZ c0 = curve.Evaluate(curve.GetEndParameter(0), false);
            XYZ c1 = curve.Evaluate(curve.GetEndParameter(1), false);
            XYZ curveParamDir = (c1 - c0).Normalize();

            bool forward = curveParamDir.DotProduct(travelDir) > 0;
            double dist = step - carryIn;
            //double clearance = _nodeClearance;
            //if (NodeFilter.IsCornerNode(from)) clearance *= 1.5;
            //if (NodeFilter.IsCornerNode(nextNode)) clearance *= 1.5;

            while (dist <= len - 1e-6)
            {
                double param = GetParameterAtLength(curve, dist);
                XYZ p = curve.Evaluate(param, false);

                XYZ tangent = curve.ComputeDerivatives(param, false).BasisX.Normalize();
                if (!forward) tangent = -tangent;

                yield return (p, curve, param, tangent, 0.0);

                dist += step;
            }

            double lastPointDist = dist - step;
            double carryOut;

            if (lastPointDist < 0)
            {
                carryOut = carryIn + len; 
                carryOut %= step;
            }
            else
            {
                carryOut = len - lastPointDist;
                carryOut %= step;
            }

            

            foreach (var nextEdge in nextNode.Edges)
            {
                if (_visitedSegments.Contains(nextEdge.Segment))
                    continue;

                foreach (var p in WalkEdge(nextEdge, nextNode, from, step, carryOut))
                    yield return p;

            }
        }

        private double GetParameterAtLength(Curve curve, double targetLength)
        {
            int divisionsPerSegment = 30;
            if (targetLength <= 0) return curve.GetEndParameter(0);
            if (targetLength >= curve.Length) return curve.GetEndParameter(1);

            double startParam = curve.GetEndParameter(0);
            double endParam = curve.GetEndParameter(1);
            double totalLength = curve.Length;

            double low = startParam;
            double high = endParam;

            for (int iter = 0; iter < 15; iter++)
            {
                double mid = (low + high) / 2;
                double lengthAtMid = GetSegmentLength(curve, startParam, mid, divisionsPerSegment);

                if (Math.Abs(lengthAtMid - targetLength) < 0.1)
                    return mid;

                if (lengthAtMid < targetLength)
                    low = mid;
                else
                    high = mid;
            }

            return (low + high) / 2;
        }

        private double GetSegmentLength(Curve curve, double startParam, double endParam, int divisions)
        {
            if (divisions < 1) divisions = 1;
            if (Math.Abs(endParam - startParam) < 1e-9)
                return 0.0;

            double totalLength = 0.0;
            XYZ prevPoint = curve.Evaluate(startParam, false);

            for (int i = 1; i <= divisions; i++)
            {
                double t = (double)i / divisions;
                double param = startParam + t * (endParam - startParam);
                XYZ point = curve.Evaluate(param, false);
                totalLength += point.DistanceTo(prevPoint);
                prevPoint = point;
            }

            return totalLength;
        }
    }
}
