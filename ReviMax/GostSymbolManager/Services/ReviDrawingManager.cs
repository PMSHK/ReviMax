using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Models.Graph;
using ReviMax.GostSymbolManager.Models.Revit;
using ReviMax.GostSymbolManager.Models.Graph.Filter;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Parameters;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.Revit.Config.Storage;

namespace ReviMax.GostSymbolManager.Services
{
    internal class ReviDrawingManager
    {
        private Document _doc;
        private CableSystemManager _cableSystemManager;
        private FamiliesPlacingManager _familiesManager;
        private RevitParametersManager _revitManager = new();
        public Document Doc { get => _doc; set => _doc = value; }
        public ReviDrawingManager(Document doc)
        {
            if (doc == null)
            {
                ReviMaxLog.Warning($"ReviDrawingManager. Document {nameof(doc)} cannot be null");
                throw new ArgumentNullException(nameof(doc), "Document cannot be null.");
            }
            _doc = doc;
            _cableSystemManager = new(doc);
            _familiesManager = new(doc);
        }

        public void DrawDetailLine(View activeView, AxisSegment curveSegment, string runId, int categoryId)
        {
            GraphicsStyle? gs = null;
            Curve curve = curveSegment.Axis;
            ReviMaxLog.Information("ReviDrawingManager. Started Drawing line.");
            Doc.StartTransaction("Draw Detail Line", doc =>
            {
                ReviMaxLog.Information("ReviDrawingManager. Starting transaction to draw detail line.");
                var detailCurve = doc.Create.NewDetailCurve(activeView, curve);
                ReviMaxStorage.Stamp(
                    detailCurve,
                    runId,
                    detailCurve.GetType().Name,
                    curveSegment.Element.Id,
                    activeView.Id
                    );
                ReviMaxLog.Information($"ReviDrawingManager. Detail line created and stamped with metadata. accepting CategoryID {categoryId}");
                Category category = Category.GetCategory(Doc, new ElementId(categoryId));
                ReviMaxLog.Information($"ReviDrawingManager. Retrieved category: {category.Name} for CategoryID {categoryId}");
                gs = category.GetGraphicsStyle(GraphicsStyleType.Projection);
                detailCurve.LineStyle = gs;
                
            });
            ReviMaxLog.Information("ReviDrawingManager. Detail line drawn successfully.");

        }

        public void DrowFamilyInstanceSymbol(XYZ point, FamilySymbol symbol, Color color, View activeView, GraphNode node, string runId) 
        {
            Doc.StartTransaction("Draw Family Instance Symbol", doc =>
            {
                if (!symbol.IsActive)
                {
                    symbol.Activate();
                    Doc.Regenerate();
                }
                
                FamilyInstance instance= Doc.Create.NewFamilyInstance(point, symbol, activeView);
                ReviMaxLog.Information("ReviDrawingManager. Family instance category: " + instance.Category.Name);

                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);

                doc.ActiveView.SetElementOverrides(instance.Id, ogs);

                ReviMaxStorage.Stamp(
                    instance,
                    runId,
                    symbol.GetType().Name,
                    node.Element.Id,
                    activeView.Id
                    );
            });
        }

        public FamilySymbol? GetAnnotationFamilySymbol(string familyName)
        {
            var family = new FilteredElementCollector(Doc)
            .OfClass(typeof(Family))
            .Cast<Family>()
            .FirstOrDefault(f => f.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase));

            if (family != null)
            {
                // Получаем символ семейства
                var symbolIds = family.GetFamilySymbolIds();
                if (symbolIds.Any())
                {
                    return Doc.GetElement(symbolIds.First()) as FamilySymbol;
                }
            }
            return null;
        }



        public void DrawRunFamily(GraphRun run, FamilySymbol symbol, View activeView, string runId, ReviLine lineSettings)
        {
            XYZ rawStart = GetStartPoint(run);
            XYZ rawEnd = GetEndPoint(run);

            XYZ start = ProjectPointToViewPlane(rawStart, activeView);
            XYZ end = ProjectPointToViewPlane(rawEnd, activeView);

            XYZ dir = end - start;
            double length = dir.GetLength();

            if (length < 1e-6)
                return;

            dir = dir.Normalize();

            List<ElementId> sourceIds = new();

            Doc.StartTransaction("Draw Run Family", doc =>
            {
                if (!symbol.IsActive)
                {
                    symbol.Activate();
                    doc.Regenerate();
                }

                // Вставляем РОВНО в начало участка
                FamilyInstance instance = doc.Create.NewFamilyInstance(start, symbol, activeView);

                // Сначала длина
                _revitManager.SetFamilyParameter(instance, "Length", length);
                _revitManager.SetFamilyParameter(instance, "RM_OFFSET", lineSettings.Offset);
                _revitManager.SetFamilyParameter(instance, "RM_STEP", lineSettings.Step);
                _revitManager.SetFamilyParameter(instance, "RM_GLYPH_SIZE", lineSettings.GlyphSize);

                Parameter p = ParameterManager.GetParameter(instance, "RM_DC_COLOR");
                p.Set($"{lineSettings.Color}");
                var filterRule = RevitFilterService.CreateFilterRule(p.Id, $"{lineSettings.Color.ToString()}");
                var categories = new List<ElementId>
                    {
                    instance.Category.Id
                    };
                ParameterFilterElement filter = RevitFilterService.GetFilter(Doc, $"RM_COLOR_{lineSettings.Color.ToString()}", categories);
                var paramFilter = new ElementParameterFilter(filterRule);
                filter.SetElementFilter(paramFilter);
                var ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(ColorMapper.FromSymbolColor(lineSettings.Color));
                activeView.SetFilterOverrides(filter.Id,ogs);
                RevitFilterService.AddFilterToView(filter, activeView);

                doc.Regenerate();

                // Потом поворот вокруг ТОЙ ЖЕ точки
                RotateFreshInstance(instance, start, dir, activeView);
                ReviMaxLog.Information($"runId: {runId},placed instance: {instance.Name} | {instance.Id}, run: {string.Join(",", run.Segments.Select(seg=>seg.Element.Id))}");

                sourceIds = run.Segments.Select(seg => seg.Element.Id).ToList();

                ReviMaxStorage.Stamp(
                    instance,
                    runId,
                    symbol.Name,
                    sourceIds,
                    activeView.Id);

                RevitElementsManager.HideElementsOnView(sourceIds, activeView);
            });
            

        }

        public XYZ GetStartPoint(GraphRun run)
        {
            return GetPointForNode(run.FirstEdge.Segment, run.StartNode);
        }

        public XYZ GetEndPoint(GraphRun run)
        {
            return GetPointForNode(run.LastEdge.Segment, run.EndNode);
        }
        private XYZ GetPointForNode(AxisSegment segment, GraphNode node)
        {
            double dStart = segment.StartPoint.DistanceTo(node.Point);
            double dEnd = segment.EndPoint.DistanceTo(node.Point);

            return dStart <= dEnd ? segment.StartPoint : segment.EndPoint;
        }
        public XYZ ProjectPointToViewPlane(XYZ p, View view)
        {
            XYZ origin = view.Origin;
            XYZ normal = view.ViewDirection.Normalize();

            double dist = (p - origin).DotProduct(normal);
            return p - normal.Multiply(dist);
        }
        private double GetAngleInViewPlane(XYZ dir, View view)
        {
            XYZ up = view.UpDirection.Normalize();
            XYZ right = view.RightDirection.Normalize();

            double x = dir.DotProduct(right);
            double y = dir.DotProduct(up);

            return Math.Atan2(x, y);
        }

        private void RotateFreshInstance(FamilyInstance instance, XYZ insertPoint, XYZ dir, View view)
        {
            double angle = -GetAngleInViewPlane(dir, view);

            Line axis = Line.CreateBound(
                insertPoint,
                insertPoint + view.ViewDirection.Normalize());

            ElementTransformUtils.RotateElement(Doc, instance.Id, axis, angle);
        }

    }
}
