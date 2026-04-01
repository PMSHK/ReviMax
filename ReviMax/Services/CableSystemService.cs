using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ReviMax.Config;
using ReviMax.Core.Elements.Filters;
using ReviMax.Core.Elements.Providers.Factory;
using ReviMax.Core.Mapper;
using ReviMax.Core.Revit;
using ReviMax.Core.Utils.Converter;
using ReviMax.DTO.Annotations;
using ReviMax.Models.Annotations;
using ReviMax.Models.Graph;
using ReviMax.Models.Graph.Filter;
using ReviMax.Models.Revit;
using ReviMax.Services.Utils;

namespace ReviMax.Services
{
    internal class CableSystemService
    {
        public Document Doc { get; }
        //public CableSystemSettings Settings { get; set; } = new CableSystemSettings();
        public View ActiveView { get; set; }
        public CableSystemService(Document doc)
        {
            Doc = doc;
            ActiveView = Doc.GetActiveView();
        }
        public void DrawCableSystemSymbols(ICableSystemCategory filter, CableSystemSettings _settings, Dictionary<FamilyMode,IList<Element>>? elements = null)
        {
            ReviMaxLog.Information($"Drawing started. Settings is {_settings.ToString()}");
            string runID = GuidBuilder.CreateGuid();
            double Tolerance = Doc.Application.VertexTolerance;

            ActiveView = Doc.GetActiveView();

            ReviDrawingManager drawingManager = new(Doc);
            RevitFilterManager filterManager = new(Doc);
            CableSystemManager cableSystemManager = new(Doc);

            CableSystemSettings? Settings = _settings;

            Dictionary<FamilyMode, IList<Element>> cableSystems = new();
            if (elements != null)
            {
                cableSystems = elements;
            } else 
            {
                cableSystems = GetCableSystemsByCategory(filter);
            }
            foreach (var element in cableSystems)
            {
                var builder = new GraphBuilder(Tolerance);
                ReviMaxLog.Information("Drawing service " + string.Join(", ", _settings.LineSettings.Select(l => l.Family.FamilyMode)));
                var line = _settings.LineSettings.FirstOrDefault(line => line.Family.FamilyMode == element.Key);
                var familyId = line.Family.Family.FamilyId;
                var familyName = line.Family.Family.FamilyName;
                var categoryId = line.CategoryId;

                var list = ExtractAxes(element.Value);
                Color color = ColorMapper.FromSymbolColor(line.Color);

                //foreach (var segment in list)
                //{
                //    ReviMaxLog.Information($"CableSystemSymbolManager. Drawing detail line in view ID {ActiveView.Id} for curve. Document {Doc}");
                //    drawingManager.DrawDetailLine(ActiveView, segment, runID, categoryId);
                //}
                var nodes = builder.Build(list);
                GraphRunsExtractor extractor = new();
                var runs = extractor.ExtractRuns(nodes, ActiveView);
                var nodeFilter = new ParralelNodeFilter();
                double tol = 50.0.MillimetersToFeet();
                var filteredRuns = nodeFilter.FilterParallelDuplicateRuns(runs, ActiveView, tol, drawingManager);
                //var symbol = ExtractFamilySymbol(familyId);
                var symbol = ExtractFamilySymbolByName(familyName);
                if (symbol != null)
                {
                    foreach (var run in filteredRuns)
                    {
                        drawingManager.DrawRunFamily(run, symbol, ActiveView, runID, line);
                    }
                }
                //TraverseGraphAndPlaceSymbols(nodes, drawingManager, familyId, color, runID, Settings);
            }
        }


        public Dictionary<FamilyMode, IList<Element>> GetCableSystemsByCategory(ICableSystemCategory filter)
        {
            RevitFilterManager filterManager = new(Doc);
            return filterManager.GetCableElementsAll(filter);
        }
        public Dictionary<FamilyMode, IList<Element>> GetSelectedCableSystemsByCategory(ICableSystemCategory filter, List<Element> elements)
        {
            RevitFilterManager filterManager = new(Doc);
            return filterManager.GetCableElementsSelected(filter, elements);
        }

        private List<AxisSegment> ExtractAxes(IList<Element> elements)
        {
            AxisProviderFactory axisProviderFactory = new(Doc);
            var segments = elements
                .Where(element => element != null)
                .Select(element => (
                    Element: element,
                    Provider: axisProviderFactory.GetProvider(element)
                    ))
                .Where(tuple => tuple.Provider != null)
                .SelectMany(x =>
                {
                    return x.Provider.GextAxes(x.Element, ActiveView) ?? Enumerable.Empty<AxisSegment>();
                })
                .ToList();
            ReviMaxLog.Information($"CableSystemSymbolManager. Retrieved axis for {segments.Count} cable system elements.");
            return segments;
        }

        private void TraverseGraphAndPlaceSymbols(
            List<GraphNode> nodes,
            ReviDrawingManager drawingManager,
            string familyId, Color color, string runID,
            CableSystemSettings? _settings = null)
        {
            CableSystemSettings? Settings = _settings;
            Settings ??= new CableSystemSettings();

            if (!Settings.Filled())
            {
                if (!Settings.GeneralSettings.Filled())
                {
                    ReviMaxLog.Warning($"CableSystemSymbolManager. General settings are not filled. Skipping symbol placement. Document {Doc}");
                    TaskDialog.Show("ReviMax", "Необходимо настроить общие настройки");
                }
                else
                {
                    ReviMaxLog.Warning($"CableSystemSymbolManager. Line settings are not filled. Skipping symbol placement. Document {Doc}");
                    TaskDialog.Show("ReviMax", "Необходимо настроить тип линии");
                }
                return;
            }
            ElementId.TryParse(familyId, out ElementId ID);

            Family? family = Doc.GetElement(ID) as Family;
            FamilySymbol familySymbol = null;

            if (family != null)
            {
                familySymbol = family.GetFamilySymbolIds()
                    .Select(x => Doc.GetElement(x))
                    .OfType<FamilySymbol>()
                    .FirstOrDefault();
            }


            //var familySymbol = Doc.GetElement(ID) as FamilySymbol;
            familySymbol ??= drawingManager.GetAnnotationFamilySymbol("circle");
            ReviMaxLog.Information($"CableSystemSymbolManager. Final family symbol used: {familySymbol.Name}. Document {Doc}");



            var globalVisitedSegments = new HashSet<AxisSegment>();
            var placed = new HashSet<(int, int)>();
            var placedPts = new List<XYZ>();

            var badNodes = nodes.Where(n => n.Edges.Count >= 3).ToList();

            foreach (var node in nodes)
            {
                var unvisitedEdge = node.Edges.FirstOrDefault(e => !globalVisitedSegments.Contains(e.Segment));
                if (unvisitedEdge == null) continue;

                var walker = new PathWalker(globalVisitedSegments);

                foreach (var (point, curve, param, tangentDir) in walker.Walk(node, Settings.GeneralSettings.Step))
                {
                    var cornerNode = nodes
                        .Where(n => NodeFilter.IsCornerNode(n))
                        .Select(n => n.Point.ProjectToViewPlane(ActiveView))
                        .ToList();


                    XYZ viewPoint = point.ProjectToViewPlane(ActiveView);

                    if (cornerNode.Any(cp => cp.DistanceTo(viewPoint) < Settings.GeneralSettings.CornerClearance))
                        continue;

                    if (badNodes.Any(n => n.Point.DistanceTo(point) < Settings.GeneralSettings.NodeClearance))
                        continue;
                    var key =
                        (
                            (int)Math.Round(viewPoint.X / Settings.GeneralSettings.Snap),
                            (int)Math.Round(viewPoint.Y / Settings.GeneralSettings.Snap)
                        );

                    if (!placed.Add(key))
                        continue;
                    if (placedPts.Any(q => q.DistanceTo(viewPoint) < Settings.GeneralSettings.MinDist))
                        continue;

                    XYZ placePoint = XYZCalculator.NormalizePointByTangentNormalFixedSide(
                        ActiveView, viewPoint, tangentDir, Settings.GeneralSettings.Offset);

                    placedPts.Add(viewPoint);

                    drawingManager.DrowFamilyInstanceSymbol(placePoint, familySymbol, color, ActiveView, node, runID);
                }
            }
        }

        public void DeleteCableSystemSymbolsByID()
        {
            CleanupManager cleanupManager = new(Doc);
            var uIDocument = new UIDocument(Doc);
            View activeView = Doc.GetActiveView();
            Reference myRef = uIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Выберите элемент для удаления всей группы");
            ElementId elementId = myRef.ElementId;

            var selectedElement = Doc?.GetElement(elementId);
            if (selectedElement != null)
            {
                string runId = ReviMaxStorage.GetRunId(selectedElement);
                if (runId != null)
                {
                    int num = cleanupManager.DeleteReviMaxElement(runId, activeView.Id);
                    ReviMaxLog.Information($"Удалено {num} символов кабельных систем с RunId: {runId} на виде {activeView.Name}");
                    TaskDialog.Show("ReviMax", "Команда удаления символов успешно выполнена.");
                }
            }
        }

        private FamilySymbol? ExtractFamilySymbol(string familyId)
        {
            ElementId.TryParse(familyId, out ElementId ID);

            Family? family = Doc.GetElement(ID) as Family;
            FamilySymbol familySymbol = null;

            if (family != null)
            {
                familySymbol = family.GetFamilySymbolIds()
                    .Select(x => Doc.GetElement(x))
                    .OfType<FamilySymbol>()
                    .FirstOrDefault();
            }
            return familySymbol;
        }

        public FamilySymbol? ExtractFamilySymbolByName(string familyName) 
        {
            var service = new FamilyService(Doc);
            var familySymbol = service.FindFamilySymbolByName(familyName);
            if (familySymbol != null) { return familySymbol; }
            return null;
        }
    }

}
