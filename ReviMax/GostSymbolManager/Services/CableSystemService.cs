using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ReviMax.CircuitAnalyzeManager.Services;
using ReviMax.Core.Config;
using ReviMax.Core.Enums;
using ReviMax.Core.Filters;
using ReviMax.Core.Providers.Factory;
using ReviMax.Core.Utils.Converter;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Models;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Models.Graph;
using ReviMax.GostSymbolManager.Models.Graph.Filter;
using ReviMax.GostSymbolManager.Models.Revit;
using ReviMax.Revit.Calculators;
using ReviMax.Revit.Config.Storage;
using ReviMax.Revit.Config.Storage.Model;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Model;

namespace ReviMax.GostSymbolManager.Services
{
    internal class CableSystemService
    {
        public Document Doc { get; }
        public View ActiveView { get; set; }
        public CableSystemService(Document doc)
        {
            Doc = doc;
            ActiveView = Doc.GetActiveView();
        }
        public void DrawCableSystemSymbols(ICableSystemCategory filter, CableSystemSettings _settings, List<DocElementsInfo> elements)
        {
            ReviMaxLog.Information($"Drawing started. Settings is {_settings.ToString()}");
            string runID = GuidBuilder.CreateGuid();
            double Tolerance = Doc.Application.VertexTolerance;

            ActiveView = Doc.GetActiveView();

            ReviDrawingManager drawingManager = new(Doc);
            RevitFilterManager filterManager = new(Doc);
            CableSystemManager cableSystemManager = new(Doc);

            CableSystemSettings? Settings = _settings;

            List<DocElementsInfo> cableSystems = elements;
            foreach (var elementSet in cableSystems)
            {
                Document? currDoc = elementSet?.DocInfo?.Document;
                if (currDoc == null) continue;
                var documentGUID = GuidBuilder.CreateVersion5Guid(ProjectInfoManager.GetDocumentIdentificationString(currDoc)).ToString();
                foreach (var element in elementSet?.Elements)
                {
                    var builder = new GraphBuilder(Tolerance);
                    ReviMaxLog.Information("Drawing service " + string.Join(", ", _settings.DocLineSettings
                        .Where(kvp=> documentGUID != null && kvp.Equals(documentGUID))
                        .SelectMany(kvp=> kvp.Value)
                        .Select(line=>line.Family.FamilyMode)));
                    var line = _settings.DocLineSettings
                        .Where(kvp => documentGUID != null && kvp.Key== documentGUID)
                        .SelectMany(kvp=>kvp.Value)
                        .FirstOrDefault(line => line.Family.FamilyMode == element.Key);
                    var familyId = line.Family.Family.FamilyId;
                    var familyName = line.Family.Family.FamilyName;
                    var categoryId = line.CategoryId;

                    SymbolColor additionalColor = line.Color;

                    var list = ExtractAxes(element.Value);

                    var nodes = builder.Build(list);
                    GraphRunsExtractor extractor = new();
                    var runs = extractor.ExtractRuns(nodes, ActiveView);
                    var nodeFilter = new ParralelNodeFilter();
                    double tol = 50.0.MillimetersToFeet();
                    var filteredRuns = nodeFilter.FilterParallelDuplicateRuns(runs, ActiveView, tol, drawingManager);

                    var symbol = ExtractFamilySymbolByName(familyName);
                    ReviMaxLog.Information(
                        $"Drawing {elementSet.Type} {element.Key}. Elements: {element.Value.Count}, " +
                        $"axes: {list.Count}, nodes: {nodes.Count}, runs: {runs.Count}, " +
                        $"filtered runs: {filteredRuns.Count}, symbol: {(symbol == null ? "<null>" : symbol.Name)}");

                    if (symbol != null)
                    {
                        foreach (var run in filteredRuns)
                        {
                            
                            drawingManager.DrawRunFamily(run, symbol, ActiveView, runID, line, additionalColor, elementSet.Type);
                        }
                    }
                }
            }
        }


        public Dictionary<FamilyMode, IList<Element>> GetCableSystemsByCategory(ICableSystemCategory filter, Document? doc = null)
        {
            var targetDocInfo = doc ?? Doc;
            RevitFilterManager filterManager = new (targetDocInfo);
            return filterManager.GetCableElementsAll(filter,doc);
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
                ReviMaxLog.Information($"Selected element ID: {elementId}. Retrieved RunId: {runId}. Document {Doc}");
                if (runId != null)
                {
                    //int num = cleanupManager.DeleteReviMaxElement(runId, activeView.Id);
                    var elementsInfoToDelete = ReviMaxStorage.GetInstanceByActiveView(activeView);
                    int num = cleanupManager.DeleteReviMaxElementsByRunId(runId, elementsInfoToDelete);

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

        public void RedrawCableSystems(Dictionary<string, List<StoredInstanceInfo>> groupedStroredInfo, CableSystemSettings settings, ICableSystemCategory currentFilter, bool enableLinked)
        {
            CleanupManager cleanupManager = new(Doc);
            foreach (var group in groupedStroredInfo)
            {
                string runID = group.Key;
                if (group.Value == null || group.Value.Count == 0) continue;

                ReviMaxLog.Information(
                    $"Redraw run {runID}. Stored instances: {group.Value.Count}. " +
                    $"Current stored: {group.Value.Count(info => info.DocumentType == RMDocumentType.CURRENT)}, " +
                    $"Linked stored: {group.Value.Count(info => info.DocumentType == RMDocumentType.LINKED)}, " +
                    $"Source ids: {string.Join(",", group.Value.SelectMany(info => info.SourceIds ?? []).Select(id => id.IntegerValue).Distinct())}");

                var currentInfos = group.Value
                    .Where(info => info.DocumentType == RMDocumentType.CURRENT)
                    .Where(info => info.SourceIds != null)
                    .ToList();

                var currentSourceIds = currentInfos
                    .SelectMany(info => info.SourceIds)
                    .Distinct()
                    .ToList();

                var currentSourceUniqueIds = currentInfos
                    .Where(info => info.SourceUniqueIds != null)
                    .SelectMany(info => info.SourceUniqueIds)
                    .Where(uniqueId => !string.IsNullOrWhiteSpace(uniqueId))
                    .Distinct()
                    .ToList();

                var currentElements = ResolveCurrentElements(currentSourceUniqueIds, currentSourceIds);

                ReviMaxLog.Information(
                    $"Redraw run {runID}. Current unique ids before cleanup: {string.Join(",", currentSourceUniqueIds)}");

                ReviMaxLog.Information(
                    $"Redraw run {runID}. Current source ids before cleanup: {string.Join(",", currentSourceIds.Select(id => id.IntegerValue))}. " +
                    $"Resolved current elements: {currentElements.Count}. " +
                    $"Resolved ids: {string.Join(",", currentElements.Select(element => element.Id.IntegerValue))}");

                var redrawFilter = GetRedrawFilter(group.Value, settings, currentElements, currentFilter);

                cleanupManager.DeleteReviMaxElement(runID, new ElementId(group.Value[0].ViewId));

                var elementsToDraw = new List<DocElementsInfo>();

                if (currentElements.Count > 0)
                {
                    var elementIds = currentElements.Select(el => el.Id).ToList();

                    TransactionManager.StartTransaction(
                        Doc,
                        "showingElements",
                        doc => RevitElementsManager.ShowElementsOnView(elementIds, doc.ActiveView));

                    var groupedElements = GroupElementsByFilter(currentElements, redrawFilter);

                    ReviMaxLog.Information(
                        $"Redraw run {runID}. Current grouped elements: " +
                        $"{string.Join(", ", groupedElements.Select(grouped => $"{grouped.Key}:{grouped.Value.Count}"))}");

                    if (groupedElements.Count > 0)
                    {
                        elementsToDraw.Add(new DocElementsInfo
                        {
                            DocInfo = new DocumentInfo(Doc),
                            Elements = groupedElements,
                            Type = RMDocumentType.CURRENT,
                        });
                    }
                }

                if (enableLinked) 
                { 
                    var collector = new ElementToDrawCollector(this);
                    var linkedElementsToDraw = collector.GetLinkedElementsToDraw(Doc, redrawFilter);
                    elementsToDraw.AddRange(linkedElementsToDraw);
                }

                ReviMaxLog.Information(
                    $"Redraw run {runID}. Elements to draw: " +
                    $"{string.Join(", ", elementsToDraw.Select(set => $"{set.Type}:{set.Elements?.Sum(grouped => grouped.Value.Count) ?? 0}"))}");

                if (elementsToDraw.Count == 0)
                    continue;

                DrawCableSystemSymbols(redrawFilter, settings, elementsToDraw);

                if (currentElements.Count > 0)
                {
                    var elementIds = currentElements.Select(el => el.Id).ToList();

                    TransactionManager.StartTransaction(
                        Doc,
                        "hidingElements",
                        doc => RevitElementsManager.HideElementsOnView(elementIds, doc.ActiveView));
                }
            }
        }

        private List<Element> ResolveCurrentElements(List<string> uniqueIds, List<ElementId> elementIds)
        {
            var result = new List<Element>();

            if (uniqueIds != null && uniqueIds.Count > 0)
            {
                result.AddRange(uniqueIds
                    .Select(uniqueId => Doc.GetElement(uniqueId))
                    .Where(element => element != null)
                    .Where(element => element.IsValidObject));
            }

            if (elementIds != null && elementIds.Count > 0)
            {
                result.AddRange(elementIds
                    .Select(id => Doc.GetElement(id))
                    .Where(element => element != null)
                    .Where(element => element.IsValidObject));
            }

            return result
                    .GroupBy(el => el.Id.IntegerValue)
                    .Select(g => g.First())
                    .ToList();
        }

        private ICableSystemCategory GetRedrawFilter(
            List<StoredInstanceInfo> storedInfos,
            CableSystemSettings settings,
            List<Element> currentElements,
            ICableSystemCategory fallbackFilter)
        {
            var modes = ResolveRedrawFamilyModes(storedInfos, settings, currentElements);

            ReviMaxLog.Information(
                $"Redraw filter modes: {(modes.Count == 0 ? "<fallback>" : string.Join(",", modes))}");

            if (modes.Contains(FamilyMode.TRAY) && modes.Contains(FamilyMode.CONDUITS))
                return new AllCableFilter();

            if (modes.Contains(FamilyMode.TRAY))
                return new CableTrayFilter();

            if (modes.Contains(FamilyMode.CONDUITS))
                return new ConduitFilter();

            return fallbackFilter ?? new AllCableFilter();
        }

        private HashSet<FamilyMode> ResolveRedrawFamilyModes(
            List<StoredInstanceInfo> storedInfos,
            CableSystemSettings settings,
            List<Element> currentElements)
        {
            var result = new HashSet<FamilyMode>();

            var symbolNames = storedInfos
                .Select(info => info.SymbolName)
                .Where(symbolName => !string.IsNullOrWhiteSpace(symbolName))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var symbolName in symbolNames)
            {
                var line = settings.DocLineSettings
                    .Where(kvp=> kvp.Key == Doc.ProjectInformation.UniqueId)
                    .SelectMany(kvp => kvp.Value)
                    .FirstOrDefault(line =>
                    string.Equals(line.Family.Family.FamilyName, symbolName, StringComparison.OrdinalIgnoreCase));

                if (line != null)
                {
                    result.Add(line.Family.FamilyMode);
                }
            }

            foreach (var element in currentElements.Where(element => element?.Category != null))
            {
                var category = element.Category.BuiltInCategory;
                if (category == BuiltInCategory.OST_CableTray || category == BuiltInCategory.OST_CableTrayFitting)
                {
                    result.Add(FamilyMode.TRAY);
                }

                if (category == BuiltInCategory.OST_Conduit || category == BuiltInCategory.OST_ConduitFitting)
                {
                    result.Add(FamilyMode.CONDUITS);
                }
            }

            return result;
        }

        private Dictionary<FamilyMode, IList<Element>> GroupElementsByFilter(List<Element> elements, ICableSystemCategory filter)
        {
            if (elements == null || elements.Count == 0 || filter == null) return [];

            var result = new Dictionary<FamilyMode, IList<Element>>();
            foreach (var group in filter.GetGroupedCategories())
            {
                var categories = group.Value.ToHashSet();
                var groupElements = elements
                    .Where(element => element?.Category != null)
                    .Where(element => categories.Contains(element.Category.BuiltInCategory))
                    .ToList();

                if (groupElements.Count > 0)
                {
                    result[group.Key] = groupElements;
                }
            }

            return result;
        }
    }

}
