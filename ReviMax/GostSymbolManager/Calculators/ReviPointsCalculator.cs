using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ReviMax.GostSymbolManager.Models.Revit;

namespace ReviMax.GostSymbolManager.Calculators
{
    internal static class ReviPointsCalculator
    {
        public static ReviPoint FindMidPoint(this IList<ReviPoint> points)
        {
            if (points == null || points.Count() == 0)
                throw new ArgumentException("Points collection is null or empty", nameof(points));
            double xMin = Double.MaxValue, yMin = Double.MaxValue, zMin = Double.MaxValue;
            double xMax = Double.MinValue, yMax = Double.MinValue, zMax = Double.MinValue;
            foreach (ReviPoint point in points) 
            {
                xMin = Math.Min(point.X,xMin);
                xMax = Math.Max(point.X,xMax);
                yMin = Math.Min(point.Y,yMin);
                yMax = Math.Max(point.Y,yMax);
                zMin = Math.Min(point.Z,zMin);
                zMax = Math.Max(point.Z,zMax);
            }
            return new ReviPoint((xMin + xMax) / 2, (yMin + yMax) / 2, (zMin + zMax) / 2);
        }
    }
}
