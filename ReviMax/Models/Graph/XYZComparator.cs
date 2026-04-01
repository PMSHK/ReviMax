using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.Models.Graph
{
    internal static class XYZComparator
    {
        public static bool AreEqual (this XYZ a, XYZ b, double tolerance)
        {
            if (a == null || b == null)
                return false;
            return a.DistanceTo(b) < tolerance;
        }
    }
}
