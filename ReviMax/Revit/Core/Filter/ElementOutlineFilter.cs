using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;
using ReviMax.Core.Enums;

namespace ReviMax.Revit.Core.Filter
{
    internal class ElementOutlineFilter
    {
        public static T? ApplyFilter<T>(T element, Outline outline) where T : Element
        {
            //BoundingBoxIntersectsFilter bbFilter = new BoundingBoxIntersectsFilter(outline);
            //Element? el = new FilteredElementCollector(element.Document)
            //    .WhereElementIsNotElementType()
            //    .WherePasses(bbFilter)
            //    .FirstOrDefault(el=>el.UniqueId == element.UniqueId);
            return IsElementInOutline(element, outline) ? element : null;
        }

        public static IList<T> ApplyFilterToList<T>(List<T> elements, View view) where T : Element
        {
            ReviMaxLog.Information($"there are {elements.Count} elements before filtering");
            elements.RemoveAll(el => !IsElementInBoundingBox(el, view));
            ReviMaxLog.Information($"there are {elements.Count} elements after filtering");
            return elements;
        }

        public static IList<T> ApplyFilterToList<T>(List<T> elements, Outline outline) where T : Element
        {
            ReviMaxLog.Information($"there are {elements.Count} elements before filtering");
            elements.RemoveAll(el => !IsElementInOutline(el, outline));
            ReviMaxLog.Information($"there are {elements.Count} elements after filtering");
            return elements;
        }

        public static bool IsElementInOutline(Element element, Outline outline)
        {
            BoundingBoxXYZ? bbox = element.get_BoundingBox(null);

            ReviMaxLog.Information(
                    $"Element {element.Id.IntegerValue} = " +
                    $"X[{bbox.Min.X};{bbox.Max.X}] " +
                    $"Y[{bbox.Min.Y};{bbox.Max.Y}] " +
                    $"Z[{bbox.Min.Z};{bbox.Max.Z}]");

            if (bbox == null) return false;

            double elementMinX = Math.Min(bbox.Min.X, bbox.Max.X);
            double elementMaxX = Math.Max(bbox.Min.X, bbox.Max.X);
            double elementMinY = Math.Min(bbox.Min.Y, bbox.Max.Y);
            double elementMaxY = Math.Max(bbox.Min.Y, bbox.Max.Y);
            double elementMinZ = Math.Min(bbox.Min.Z, bbox.Max.Z);
            double elementMaxZ = Math.Max(bbox.Min.Z, bbox.Max.Z);

            if (element.Id.IntegerValue % 20 == 0)
            {
                ReviMaxLog.Information($"[DEBUG] Элемент {element.Id} ({element.GetType().Name}): " +
                                       $"X[{elementMinX:F2}, {elementMaxX:F2}], Y[{elementMinY:F2}, {elementMaxY:F2}], Z[{elementMinZ:F2}, {elementMaxZ:F2}]");
                ReviMaxLog.Information($"[DEBUG] Целевой Outline: " +
                                       $"X[{outline.MinimumPoint.X:F2}, {outline.MaximumPoint.X:F2}], " +
                                       $"Y[{outline.MinimumPoint.Y:F2}, {outline.MaximumPoint.Y:F2}], " +
                                       $"Z[{outline.MinimumPoint.Z:F2}, {outline.MaximumPoint.Z:F2}]");
            }

            bool separatedX = elementMaxX < outline.MinimumPoint.X || elementMinX > outline.MaximumPoint.X;
            bool separatedY = elementMaxY < outline.MinimumPoint.Y || elementMinY > outline.MaximumPoint.Y;
            //bool separatedZ = elementMaxZ < outline.MinimumPoint.Z || elementMinZ > outline.MaximumPoint.Z;

            return !(separatedX || separatedY );
            //return !(bbox.Max.X < outline.MinimumPoint.X || bbox.Min.X > outline.MaximumPoint.X ||
            //         bbox.Max.Y < outline.MinimumPoint.Y || bbox.Min.Y > outline.MaximumPoint.Y ||
            //         bbox.Max.Z < outline.MinimumPoint.Z || bbox.Min.Z > outline.MaximumPoint.Z);
        }

        public static bool IsElementInBoundingBox(Element element, View view)
        {
            BoundingBoxXYZ elementBox = element.get_BoundingBox(null);
            BoundingBoxXYZ viewBox = view.get_BoundingBox(view);
            if (elementBox == null || viewBox == null) return false;

            double elementMinX = Math.Min(elementBox.Min.X, elementBox.Max.X);
            double elementMaxX = Math.Max(elementBox.Min.X, elementBox.Max.X);
            double elementMinY = Math.Min(elementBox.Min.Y, elementBox.Max.Y);
            double elementMaxY = Math.Max(elementBox.Min.Y, elementBox.Max.Y);
            double elementMinZ = Math.Min(elementBox.Min.Z, elementBox.Max.Z);
            double elementMaxZ = Math.Max(elementBox.Min.Z, elementBox.Max.Z);
            ReviMaxLog.Information($"element box has min x: {elementMinX}, min y: {elementMinY}, min z: {elementMinZ}, max x: {elementMaxX}, max y: {elementMaxY}, max z: {elementMaxZ}");
            ReviMaxLog.Information($"active View has min x: {viewBox.Min.X}, min y: {viewBox.Min.Y}, min z: {viewBox.Min.Z} , max x: {viewBox.Max.X}, max y: {viewBox.Max.Y}, max z: {viewBox.Max.Z}");

            bool intersectsX = elementMinX > viewBox.Max.X || elementMaxX < viewBox.Min.X;
            bool intersectsY = elementMinY > viewBox.Max.Y || elementMaxY < viewBox.Min.Y;
            bool intersectsZ = elementMinZ > viewBox.Max.Z || elementMaxZ < viewBox.Min.Z;
            return !(intersectsX || intersectsY || intersectsZ);
        }
    }
}
