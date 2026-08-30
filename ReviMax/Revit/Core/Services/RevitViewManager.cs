using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;
using ReviMax.Handlers;

namespace ReviMax.Revit.Core.Services
{
    internal static class RevitViewManager
    {
        //public static Outline GetOutline(View view)
        //{
        //    BoundingBoxXYZ? boundingBox = view.CropBox;
        //    if (boundingBox != null)
        //    {
        //        Transform transform = boundingBox.Transform;
        //        XYZ min = transform.OfPoint(boundingBox.Min);
        //        XYZ max = transform.OfPoint(boundingBox.Max);

        //    } 
        //}

        public static XYZ[] GetLocalVerticies(View view)
        {
            if (view == null) return [];
                View activeView = view;
                BoundingBoxXYZ cropBox = activeView.CropBox;
                XYZ[] localVertices = new XYZ[4]
                {
                    new XYZ(cropBox.Min.X, cropBox.Min.Y, 0),
                    new XYZ(cropBox.Max.X, cropBox.Min.Y, 0),
                    new XYZ(cropBox.Max.X, cropBox.Max.Y, 0),
                    new XYZ(cropBox.Min.X, cropBox.Max.Y, 0)
                };
            return localVertices;
        }

        public static XYZ[] TransformVerticies(Transform transform, XYZ[] localVerticies)
        {
            return localVerticies.Select(v => transform.OfPoint(v)).ToArray();
        }

        public static (XYZ min, XYZ max) GetWorldLimits(XYZ[] worldVertices) 
        {
            XYZ worldMin = new XYZ(worldVertices.Min(v => v.X), worldVertices.Min(v => v.Y), worldVertices.Min(v => v.Z));
            XYZ worldMax = new XYZ(worldVertices.Max(v => v.X), worldVertices.Max(v => v.Y), worldVertices.Max(v => v.Z));
            return (worldMin, worldMax);
        }
        public static (XYZ min, XYZ max) GetWorldLimits(XYZ[] worldVertices, (double minZ, double maxZ) zCoordinate)
        {
            XYZ worldMin = new XYZ(worldVertices.Min(v => v.X), worldVertices.Min(v => v.Y), zCoordinate.minZ);
            XYZ worldMax = new XYZ(worldVertices.Max(v => v.X), worldVertices.Max(v => v.Y), zCoordinate.maxZ);
            return (worldMin, worldMax);
        }

        public static (XYZ min, XYZ max) GetWorldLimits(View view) 
        {
            var bBox = view.CropBox;
            XYZ viewDir = view.ViewDirection; 
            XYZ upDir = view.UpDirection;     
            XYZ rightDir = view.RightDirection; 
            XYZ origin = view.Origin;         

            XYZ min = bBox.Min;
            XYZ max = bBox.Max;

            XYZ[] worldVertices = new XYZ[]
            {
                origin + min.X * rightDir + min.Y * upDir + min.Z * viewDir,
                origin + max.X * rightDir + min.Y * upDir + min.Z * viewDir,
                origin + max.X * rightDir + max.Y * upDir + min.Z * viewDir,
                origin + min.X * rightDir + max.Y * upDir + min.Z * viewDir,
                origin + min.X * rightDir + min.Y * upDir + max.Z * viewDir,
                origin + max.X * rightDir + min.Y * upDir + max.Z * viewDir,
                origin + max.X * rightDir + max.Y * upDir + max.Z * viewDir,
                origin + min.X * rightDir + max.Y * upDir + max.Z * viewDir
            };

            XYZ worldMin = new XYZ(worldVertices.Min(v => v.X), worldVertices.Min(v => v.Y), worldVertices.Min(v => v.Z));
            XYZ worldMax = new XYZ(worldVertices.Max(v => v.X), worldVertices.Max(v => v.Y), worldVertices.Max(v => v.Z));

            ReviMaxLog.Information($"WORLD Outline = X[{worldMin.X};{worldMax.X}] Y[{worldMin.Y};{worldMax.Y}] Z[{worldMin.Z};{worldMax.Z}]");
            
            return (worldMin, worldMax);
        }
        public static PlanViewRange? GetPlanViewRange(View view)
        {
            var viewPlan = view as ViewPlan;
            if (viewPlan == null) { }

            PlanViewRange? planViewRange = viewPlan?.GetViewRange();
            return planViewRange;
        }
        public static (double minZ, double maxZ) GetPlanViewRangeZ(View view)
        {
            var planViewRange = GetPlanViewRange(view);
            var bottomLevelId = planViewRange?.GetLevelId(PlanViewPlane.BottomClipPlane);
            var topLevelId = planViewRange?.GetLevelId(PlanViewPlane.TopClipPlane);

            var bottomLevel = bottomLevelId != null ? view.Document.GetElement(bottomLevelId) as Level : null;
            var topLevel = topLevelId != null ? view.Document.GetElement(topLevelId) as Level : null;

            double worldBottomZ = bottomLevel != null && planViewRange!= null ? bottomLevel.Elevation + planViewRange.GetOffset(PlanViewPlane.BottomClipPlane): -1000.0;
            double worldTopZ = topLevel != null && planViewRange!= null ? topLevel.Elevation + planViewRange.GetOffset(PlanViewPlane.TopClipPlane): 1000.0;
            return (worldBottomZ, worldTopZ);
        }
    }
}
