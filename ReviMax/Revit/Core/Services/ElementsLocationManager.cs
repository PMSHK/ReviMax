using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;

namespace ReviMax.Revit.Core.Services
{
    public class ElementsLocationManager
    {
        public static XYZ? GetElementLocation(Element element)
        {
            if (element == null || !element.IsValidObject)
            {
                ReviMaxLog.Warning($"Element is null or invalid object");
                return null;
            }

            try
            {
                // Получаем BoundingBox в координатах модели (передаем null вместо View)
                BoundingBoxXYZ bbox = element.get_BoundingBox(null);

                if (bbox != null)
                {
                    // Вычисляем центр габаритного контейнера
                    XYZ min = bbox.Min;
                    XYZ max = bbox.Max;
                    XYZ center = new XYZ((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);

                    ReviMaxLog.Information($"Got 3D center for element {element.Name}: {center}");
                    return center;
                }
                else
                {
                    ReviMaxLog.Warning($"BoundingBox is null for {element.Name}. Falling back to Location.");
                    return GetLegacyLocation(element);
                }
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Failed to get 3D center: {ex.Message}");
                return null;
            }

        }


        private static XYZ? GetLegacyLocation(Element element)
        {
            XYZ? point = default;
            try
            {
                if (element.Location is LocationPoint locationPoint)
                {
                    point = locationPoint.Point;
                }
                else if (element.Location is LocationCurve locationCurve)
                {
                    var curve = locationCurve.Curve;
                    if (curve != null && curve.IsBound)
                    {
                        point = curve.GetEndPoint(0);
                    }
                }
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Failed to get legacy location: {ex.Message}");
            }
            return point;
        }

    }
}
