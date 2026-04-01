using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Services;

namespace ReviMax.Models.Graph.Filter
{
    internal class ParralelNodeFilter
    {
        public List<GraphRun> FilterParallelDuplicateRuns(
    List<GraphRun> runs,
    View view,
    double distanceTolerance,
    ReviDrawingManager drawingManager)
        {
            var accepted = new List<RunGeometry>();

            foreach (var run in runs)
            {
                var geom = BuildRunGeometry(run, view, drawingManager);
                if (geom.Length < 1e-6)
                    continue;

                bool isDuplicate = accepted.Any(a => AreDuplicateRuns(a, geom, distanceTolerance));
                if (!isDuplicate)
                    accepted.Add(geom);
            }

            return accepted.Select(a => a.Run).ToList();
        }

        private bool AreDuplicateRuns(RunGeometry a, RunGeometry b, double distanceTolerance)
        {
            if (a.Dir == XYZ.Zero || b.Dir == XYZ.Zero)
                return false;

            // 1. Почти параллельны
            double cosTol = Math.Cos(3.0 * Math.PI / 180.0); // 3 градуса
            double dot = Math.Abs(a.Dir.DotProduct(b.Dir));
            if (dot < cosTol)
                return false;

            // 2. Средняя точка одного участка близка к линии другого
            double distMid = DistancePointToSegment(b.Mid, a.Start, a.End);
            if (distMid > distanceTolerance)
                return false;

            // 3. Есть перекрытие проекций вдоль направления
            if (!HaveProjectionOverlap(a, b))
                return false;

            return true;
        }

        private double DistancePointToSegment(XYZ p, XYZ a, XYZ b)
        {
            XYZ ab = b - a;
            double ab2 = ab.DotProduct(ab);
            if (ab2 < 1e-9)
                return p.DistanceTo(a);

            double t = (p - a).DotProduct(ab) / ab2;
            t = Math.Max(0.0, Math.Min(1.0, t));

            XYZ proj = a + ab.Multiply(t);
            return p.DistanceTo(proj);
        }
        private bool HaveProjectionOverlap(RunGeometry a, RunGeometry b)
        {
            XYZ axis = a.Dir;

            double a0 = 0.0;
            double a1 = a.Length;

            double b0 = (b.Start - a.Start).DotProduct(axis);
            double b1 = (b.End - a.Start).DotProduct(axis);

            double minB = Math.Min(b0, b1);
            double maxB = Math.Max(b0, b1);

            double overlap = Math.Min(a1, maxB) - Math.Max(a0, minB);

            return overlap > 1e-6;
        }
        private RunGeometry BuildRunGeometry(GraphRun run, View view, ReviDrawingManager drawingManager)
        {
            XYZ start = drawingManager.ProjectPointToViewPlane(drawingManager.GetStartPoint(run), view);
            XYZ end = drawingManager.ProjectPointToViewPlane(drawingManager.GetEndPoint(run), view);

            XYZ v = end - start;
            double len = v.GetLength();
            XYZ dir = len < 1e-9 ? XYZ.Zero : v.Normalize();

            return new RunGeometry
            {
                Run = run,
                Start = start,
                End = end,
                Dir = dir,
                Length = len
            };
        }
    }
}
