using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.Core.Revit
{
    internal static class ReviPoint
    {
        public static XYZ ProjectToViewPlane(this XYZ point, View view)
        {
            Plane plane = Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
            
            XYZ vectorToPoint = point - plane.Origin;
            double distance = vectorToPoint.DotProduct(plane.Normal);
            XYZ projectedPoint = point - distance * plane.Normal;
            return projectedPoint;
        }
    }
}
