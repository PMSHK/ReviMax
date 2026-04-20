using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Enums;

namespace ReviMax.Revit.Core.Services
{
    internal class XYZCompareService
    {
        public static CoordinateDirection Direction { get; set; }

        public static Comparison<XYZ> Comparer => (a, b) =>
        {
            int result = Direction switch
            {
                CoordinateDirection.X => XComparer(a, b),
                CoordinateDirection.Y => YComparer(a, b),
                CoordinateDirection.Z => ZComparer(a, b),
                _ => 0
            };
            return result;
        };

        public static Comparison<Element> ElementComparer => (a, b) =>
        {
            //XYZ aPoint = a.Location as LocationPoint != null ? (a?.Location as LocationPoint).Point : XYZ.Zero;
            //XYZ bPoint = b.Location as LocationPoint != null ? (b?.Location as LocationPoint).Point : XYZ.Zero;
            XYZ aPoint = GetPoint(a);
            XYZ bPoint = GetPoint(b);
            int result = Direction switch
            {
                CoordinateDirection.X => XComparer(aPoint , bPoint),
                CoordinateDirection.Y => YComparer(aPoint, bPoint),
                CoordinateDirection.Z => ZComparer(aPoint, bPoint),
                _ => 0
            };
            return result;
        };

        private static int XComparer(XYZ a, XYZ b)
        {
            int result = CompareDouble(a.X, b.X, 1e-6);
            if (result != 0) return result;
            result = CompareDouble(a.Y, b.Y, 1e-6);
            if (result != 0) return result;
            return CompareDouble(a.Z, b.Z, 1e-6);
        }
        private static int YComparer(XYZ a, XYZ b)
        {
            int result = CompareDouble(a.Y, b.Y, 1e-6);
            if (result != 0) return result;
            result = CompareDouble(a.X, b.X, 1e-6);
            if (result != 0) return result;
            return CompareDouble(a.Z, b.Z, 1e-6);
        }
        private static int ZComparer(XYZ a, XYZ b)
        {
            int result = CompareDouble(a.Z, b.Z, 1e-6);
            if (result != 0) return result;
            result = CompareDouble(a.X, b.X, 1e-6);
            if (result != 0) return result;
            return CompareDouble(a.Y, b.Y, 1e-6);
        }

        private static XYZ GetPoint(Element element)
        {
            if (element == null)
                return XYZ.Zero;

            if (element.Location is LocationPoint lp)
                return lp.Point;

            if (element.Location is LocationCurve lc)
                return lc.Curve.GetEndPoint(0);

            BoundingBoxXYZ bb = element.get_BoundingBox(null);
            if (bb != null)
            {
                return new XYZ(
                    (bb.Min.X + bb.Max.X) / 2.0,
                    (bb.Min.Y + bb.Max.Y) / 2.0,
                    (bb.Min.Z + bb.Max.Z) / 2.0
                );
            }

            return XYZ.Zero;
        }

        private static int CompareDouble(double a, double b, double tolerance)
        {
            if (Math.Abs(a - b) < tolerance)
                return 0;

            return a < b ? -1 : 1;
        }

        private double GetDirectionValue(XYZ xyz)
        {
            return Direction switch
            {
                CoordinateDirection.X => xyz.X,
                CoordinateDirection.Y => xyz.Y,
                CoordinateDirection.Z => xyz.Z,
                _ => throw new NotImplementedException($"Direction {Direction} not implemented")
            };
        }
    }
}
