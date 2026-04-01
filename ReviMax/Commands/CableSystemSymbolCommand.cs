using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using ReviMax.Config;
using ReviMax.core.Elements;
using ReviMax.Core.Elements.Filters;
using ReviMax.Core.Elements.Providers.Factory;
using ReviMax.Core.Revit;
using ReviMax.Core.Utils.Converter;
using ReviMax.Models.Graph;
using ReviMax.Models.Graph.Filter;
using ReviMax.Models.Revit;
using ReviMax.Services;
using ReviMax.Services.Utils;

namespace ReviMax.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CableSystemSymbolCommand : IExternalCommand
    {

        public CableSystemSymbolCommand() { }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            View _activeView;
            Document Doc = commandData.Application.ActiveUIDocument.Document;
            string runID = GuidBuilder.CreateGuid();
            double Tolerance = Doc.Application.VertexTolerance;
            try
            {
                _activeView = Doc.GetActiveView();
            }
            catch (Exception e)
            {
                ReviMaxLog.Error($"Cannot get active view: {e.Message}");
                return Result.Failed;
            }

            ReviDrawingManager drawingManager = new(Doc);
            RevitFilterManager filterManager = new(Doc);
            CableSystemManager cableSystemManager = new(Doc);

            List<Category> lineStyles = filterManager.GetLineStyles();
            IList<Element> cableSystems = filterManager.GetCableElementsByCategory(new CableTrayFilter());
            //IList<Element> cableSystems = filterManager.GetCableElementsByCategory(BuiltInCategory.OST_CableTray);
            AxisProviderFactory axisProviderFactory = new(Doc);
            List<AxisSegment> segments = cableSystems
                .Where(element => element != null)
                .Select(element => (
                    Element: element,
                    Provider: axisProviderFactory.GetProvider(element)
                    ))
                .Where(tuple => tuple.Provider != null)
                .SelectMany(x =>
                    {
                        return x.Provider.GextAxes(x.Element, _activeView) ?? Enumerable.Empty<AxisSegment>();
                    })
                .ToList();
            ReviMaxLog.Information($"CableSystemSymbolManager. Retrieved axis for {segments.Count} cable system elements.");
            foreach (var segment in segments)
            {
                ReviMaxLog.Information($"CableSystemSymbolManager. Drawing detail line in view ID {_activeView.Id} for curve. Document {Doc}");
                drawingManager.DrawDetailLine(_activeView, segment, runID,78);
            }
            var builder = new GraphBuilder(Tolerance);
            var nodes = builder.Build(segments);
            double step = 500.0 / 304.8; // 0.5 meter step
            double offset = 500 / 304.8; // 500 mm offset
            double snap = 2.0 / 304.8;          // 2 мм
            double minDist = 300.0 / 304.8;      // 300 мм анти-скопление
            double nodeClear = 250.0 / 304.8;    // 250 мм вокруг разветвителей
            double cornerClear = 400.0 / 304.8;  // 400 мм от углов
            var family = drawingManager.GetAnnotationFamilySymbol("circle");

            var globalVisitedSegments = new HashSet<AxisSegment>();
            var placed = new HashSet<(int, int)>();
            var placedPts = new List<XYZ>();

            var badNodes = nodes.Where(n => n.Edges.Count >= 3).ToList();

            foreach (var node in nodes)
            {
                var unvisitedEdge = node.Edges.FirstOrDefault(e => !globalVisitedSegments.Contains(e.Segment));
                if (unvisitedEdge == null) continue;

                var walker = new PathWalker(globalVisitedSegments);

                foreach (var (point, curve, param, tangentDir) in walker.Walk(node, step))
                {
                    var cornerNode = nodes
                        .Where(n => NodeFilter.IsCornerNode(n))
                        .Select(n => n.Point.ProjectToViewPlane(_activeView))
                        .ToList();


                    XYZ viewPoint = point.ProjectToViewPlane(_activeView);

                    if (cornerNode.Any(cp => cp.DistanceTo(viewPoint) < cornerClear))
                        continue;

                    if (badNodes.Any(n => n.Point.DistanceTo(point) < nodeClear))
                        continue;
                    var key = 
                        (
                            (int)Math.Round(viewPoint.X / snap),
                            (int)Math.Round(viewPoint.Y / snap)
                        );

                    if (!placed.Add(key))
                        continue;
                    if (placedPts.Any(q => q.DistanceTo(viewPoint) < minDist))
                        continue;

                    XYZ placePoint = XYZCalculator.NormalizePointByTangentNormalFixedSide(
                        _activeView, viewPoint, tangentDir, offset);

                    placedPts.Add(viewPoint);

                    drawingManager.DrowFamilyInstanceSymbol(placePoint, family, new Color(255,0,0), _activeView, node, runID);
                }
            }

            TaskDialog.Show("ReviMax", "Команда управления символами кабельных систем запущена.");
            return Result.Succeeded;
        }
    }
}
