using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.Revit.Calculators
{
    internal static class XYZCalculator
    {
        public static XYZ NormalizePointByTangentNormal(View view, XYZ point, Curve curve, double param, double offset)
        {
            XYZ tangent = curve
        .ComputeDerivatives(param, true)
        .BasisX.Normalize();

            if (tangent.DotProduct(view.RightDirection) < 0)
                tangent = -tangent;

            XYZ normal = view.ViewDirection
                .CrossProduct(tangent)
                .Normalize();

            return point + normal * offset;
        }
        public static XYZ NormalizePointByTangentNormalFixedSide(
    View view,
    XYZ point,
    Curve curve,
    double param,
    double offset)
        {
            // Получаем тангенс в точке (в мировых координатах)
            XYZ tangent = curve.ComputeDerivatives(param, false).BasisX.Normalize();

            // Проекция тангенса на плоскость вида
            XYZ viewDir = view.ViewDirection.Normalize();
            XYZ tangentInView = tangent - (tangent.DotProduct(viewDir)) * viewDir;
            if (tangentInView.IsZeroLength())
                tangentInView = view.RightDirection; // fallback

            tangentInView = tangentInView.Normalize();

            // Правая нормаль в плоскости вида: ViewDirection × Tangent
            XYZ normal = viewDir.CrossProduct(tangentInView).Normalize();

            // ВАЖНО: НЕ инвертируем — это обеспечивает согласованность "справа по ходу"
            return point + normal * offset;
        }

        public static XYZ NormalizePointByTangentNormalFixedSide(
    View view,
    XYZ point,
    XYZ tangentWorld,
    double offset)
        {
            XYZ viewDir = view.ViewDirection.Normalize();

            // Проекция тангенса на плоскость вида
            XYZ tangentInView = tangentWorld - (tangentWorld.DotProduct(viewDir)) * viewDir;
            if (tangentInView.IsZeroLength())
                tangentInView = view.RightDirection;

            tangentInView = tangentInView.Normalize();

            // Правая нормаль в плоскости вида
            XYZ normal = viewDir.CrossProduct(tangentInView).Normalize();

            return point + normal * offset;
        }

    }


}
