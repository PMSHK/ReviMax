using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Dynamitey;
using ReviMax.Core.Config;
using ReviMax.Core.Enums;
using ReviMax.Core.Filters;
using ReviMax.Core.Providers.Factory;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Converter;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Models;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Services;
using ReviMax.GostSymbolManager.Services.Utils;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.Revit.Config.Storage;
using ReviMax.Revit.Config.Storage.Model;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Bridge.Event;
using ReviMax.Revit.Core.Filter;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Model;

namespace ReviMax.GostSymbolManager.UI.ViewModel
{
    public class CableSystemManagerVM : ViewModelBase
    {
        private readonly Document _doc;
        private CableSystemSettings _settings;
        private CableSystemFilterMode _currentFilterMode;
        private CableSystemService _cableSystemService;
        private bool _enableLinkedDocuments;
        private bool _justLinkedDocuments;
        public bool EnableLinkedDocuments
        {
            get => _enableLinkedDocuments;
            set
            {
                if (_enableLinkedDocuments != value)
                {
                    _enableLinkedDocuments = value;
                    if (value) JustLinkedDocuments = false;
                    ReviMaxLog.Information($"Enable linked documents changed to: {_enableLinkedDocuments}");
                    ManageDataTypes();
                    OnPropertyChanged();
                }
            }
        }

        public bool JustLinkedDocuments
        {
            get => _justLinkedDocuments;
            set
            {
                if (_justLinkedDocuments != value)
                {
                    _justLinkedDocuments = value;
                    if (value) 
                    {
                        EnableLinkedDocuments = false;
                    }
                    ReviMaxLog.Information($"Just linked documents changed to: {_justLinkedDocuments}");
                    ManageDataTypes();
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<CableSystemRowVM> Systems { get; } = new();

        public RevitDispatcherService dispatcher = new();
        public Dictionary<FamilyMode,IList<Element>> SelectedElements { get; set; }
        internal ICableSystemCategory? CurrentFilter {  get; set; }
        internal List<DocElementsInfo> CableSystems { get; } = new();
        public CableSystemFilterMode CurrentFilterMode 
        { 
            get => _currentFilterMode; 
            set
            {
                if(_currentFilterMode != value)
                {
                    _currentFilterMode = value;
                    ReviMaxLog.Information($"Current filter mode changed to: {_currentFilterMode}");

                    ManageDataTypes();

                    OnPropertyChanged();
                }
            }
        }



        RevitFilterManager _filterManager;
        
        public Document Doc { get => _doc; }
        public ICommand PlaceGostAnnotations { get; } 
        public ICommand RedrawGostAnnotations { get; }
        public ICommand DeleteGostAnnotations { get; }
        public ICommand SaveSettings { get; }

        public CableSystemManagerVM(Document doc)
        {
            
            _doc = doc!=null? doc : throw new ArgumentException("Document is null");
            _settings = ReviMaxSettingsProvider.Load(Doc,dispatcher);
            _filterManager = new(_doc);
            _cableSystemService = new(_doc);
            PlaceGostAnnotations = new ReviMaxCommand(PlaceGostSymbols, CanManipulateGostSymbols);
            RedrawGostAnnotations = new ReviMaxCommand(RedrawGostSymbols, () => true);
            DeleteGostAnnotations = new ReviMaxCommand(DeleteGostSymbols, CanManipulateGostSymbols);
            SaveSettings = new ReviMaxCommand(()=> SaveLoadFileService.SaveToFile<CableSystemSettingsDto>(_settings.ToDto()) ,() => _settings != null && _settings.Filled());
            ManageDataTypes();
        }

        public void PlaceGostSymbols()
        {
            var currentFilter = GetCurrentFilter();
            if (currentFilter == null) return;
            dispatcher.Request(
                request: app => { _cableSystemService.DrawCableSystemSymbols(CurrentFilter, _settings, CableSystems);
                },
                callback: () => { ReviMaxLog.Information("Placing GOST symbols..."); }
                );
                return;
        }
        public void RedrawGostSymbols() 
        {
            ReviMaxLog.Information("Redrawing GOST symbols...");
            dispatcher.Request(
                request: app => { 
                    var groupedStroredInfo = ReviMaxStorage.GetInstanceByActiveView(Doc.ActiveView);

                    if (groupedStroredInfo.Count <= 0) return;

                    var currentFilter = GetCurrentFilter();
                    if (currentFilter == null) return;

                    if (!JustLinkedDocuments)
                    {
                        _cableSystemService.RedrawCableSystems(groupedStroredInfo, _settings, currentFilter, EnableLinkedDocuments);
                    }
                    else 
                    {
                        _cableSystemService.RedrawCableSystems(groupedStroredInfo, _settings, currentFilter, true);
                    }
                    
                },
                callback: () => {
                    
                    ReviMaxLog.Information("GOST symbols redrawn."); }
                );
        }
        public void DeleteGostSymbols() 
        {

            dispatcher.Request(
                request: app => { _cableSystemService.DeleteCableSystemSymbolsByID(); },
            callback: () => { ReviMaxLog.Information("Deleting GOST symbols..."); }
            );
        }
        public bool CanManipulateGostSymbols() 
        {
            return CurrentFilterMode.GetType != null ? true : false;
        }
        public bool CanRedrawGostSymbols() 
        {
            return ReviMaxStorage.GetInstanceByActiveView(Doc.ActiveView).Count > 0;
        }
        private ICableSystemCategory? GetCurrentFilter()
        {
            var manager = new RevitFilterManager(Doc);
            CurrentFilter = _currentFilterMode switch
            {
                CableSystemFilterMode.ALL => new AllCableFilter() ,
                CableSystemFilterMode.TRAYS => new CableTrayFilter(),
                CableSystemFilterMode.CONDUITS => new ConduitFilter(),
                CableSystemFilterMode.SELECTED => CreateFilterForSelected(),
                _ => throw new InvalidOperationException("Invalid filter mode")
            };
            return CurrentFilter;
        }
        private ICableSystemCategory? CreateFilterForSelected()
        {
            RevitSelectionService selectionService = new(new UIDocument(Doc));
            var filterFactory = new RevitFilterFactory();
            var elements = new List<Element>();
            HashSet<BuiltInCategory> categories = new();
            ICableSystemCategory filter = null;
            RevitFilterManager filterManager = new(Doc);
            Dictionary<FamilyMode, IList<Element>> groupedElements = new();
            dispatcher.Request(
                request: app => { 
                    var selectedElements = selectionService.GetSelectionCableSystems();
                    if (selectedElements == null || selectedElements.Count == 0) { return; }
                    categories = RevitCategoriesService.ExtractCategoriesFromElements(selectedElements);
                    filter = filterFactory.GetFilter(categories);
                    groupedElements = filterManager.GetCableElementsSelected(filter, selectedElements);
                    elements = selectedElements;
                },
                callback: () => {
                    SelectedElements = groupedElements;
                    CurrentFilter = filter!;

                    if (filter == null)
                    {
                        throw new Exception($"Filter for categories {string.Join(", ", categories.Select(c => c.ToString()))} not found");
                    }
                    ReviMaxLog.Information($"Filter selected: {filter.ToString()}");
                    var elementsToDraw = new List<DocElementsInfo>
                    {
                        new DocElementsInfo()
                        {
                        DocInfo = new DocumentInfo(Doc),
                        Elements = groupedElements,
                        }
                    };
                    LoadCableSystemRow(elementsToDraw);
                    ReviMaxLog.Information($"elements is : {string.Join(", ", elements.Select(e => e.Name))}");

                }

                );
            
            return filter;
        }
        private void ManageDataTypes()
        {
            dispatcher.Request(
                request: app =>
                {
                    ElementToDrawCollector collector = new(_cableSystemService);
                    var elementsToDraw = new List<DocElementsInfo>();
                    var currentFilter = GetCurrentFilter();
                    if (currentFilter == null) return;

                    View activeView = Doc.ActiveView;
                    BoundingBoxXYZ cropBox = activeView.CropBox;

                    (double worldMinZ, double worldMaxZ) = RevitViewManager.GetPlanViewRangeZ(activeView);
                    ReviMaxLog.Information($"worldMinZ = {worldMinZ}, worldMaxZ = {worldMaxZ}");

                    XYZ[] localVertices = RevitViewManager.GetLocalVerticies(activeView);
                    XYZ[] worldVerices = RevitViewManager.TransformVerticies(activeView.CropBox.Transform, localVertices);
                    (XYZ min, XYZ max) viewLimits = RevitViewManager.GetWorldLimits(worldVerices, (worldMinZ, worldMaxZ));

                    Outline hostOutline = new Outline(viewLimits.min, viewLimits.max);
                    var hostBoxFilter = new BoundingBoxIntersectsFilter(hostOutline);
                    ReviMaxLog.Information($"world hostOutline = X[{hostOutline.MinimumPoint.X};{hostOutline.MaximumPoint.X}] " +
                        $"Y[{hostOutline.MinimumPoint.Y};{hostOutline.MaximumPoint.Y}] " +
                        $"Z[{hostOutline.MinimumPoint.Z};{hostOutline.MaximumPoint.Z}]");

                    var viewPoints = RevitElementsManager.GetMinMaxPointsOfView(activeView, activeView);

                    if (!JustLinkedDocuments)
                    {
                        elementsToDraw.Add(collector.GetElementsToDraw(Doc, currentFilter));

                        if (EnableLinkedDocuments)
                        {
                            var linkedElementsToDraw = collector.GetLinkedElementsToDraw(Doc, currentFilter);
                            foreach (var linkedElement in linkedElementsToDraw)
                            {
                                if (linkedElement == null) continue;

                                Outline hostOutlineInLinkedSystem = new Outline(linkedElement.InverseTransform.OfPoint(viewLimits.min), linkedElement.InverseTransform.OfPoint(viewLimits.max));
                                var transformedHostBoxFilter = new BoundingBoxIntersectsFilter(hostOutlineInLinkedSystem);
                                var docElements = linkedElement?.Elements;
                                if (docElements != null)
                                {
                                    foreach (var elementList in docElements.Values)
                                    {
                                        ((List<Element>)elementList).RemoveAll(el=>!transformedHostBoxFilter.PassesFilter(el));
                                    }
                                }
                            }
                            elementsToDraw.AddRange(linkedElementsToDraw);
                        }
                    }

                    else
                    {
                        Outline outline = RevitElementsManager.BuildOutline(viewPoints.min, viewPoints.max);
                        var linkedElementsToDraw = collector.GetLinkedElementsToDraw(Doc, currentFilter);
                        elementsToDraw.AddRange(linkedElementsToDraw);
                    }

                    LoadCableSystemRow(elementsToDraw);
                },
                callback: () =>
                {
                    ReviMaxLog.Information($"Manage data types for filter mode: {_currentFilterMode}");
                });
        }
        private void LoadCableSystemRow(List<DocElementsInfo> list)
        {
            Systems.Clear();
            CableSystems.Clear();
            foreach (var docElementsInfo in list)
            {
                var currDoc = docElementsInfo?.DocInfo?.Document;
                if (currDoc == null) continue;
                var documentUniqueID = ProjectInfoManager.GetDocumentIdentificationString(currDoc);
                string documentGuid = GuidBuilder.CreateVersion5Guid(documentUniqueID).ToString();
                if (documentGuid == null) continue;
                ReviMaxLog.Information($"Document ID for Document {docElementsInfo.DocInfo.Document.Title} is: {documentGuid}");
                foreach (var element in docElementsInfo.Elements)
                {
                    var _line = new ReviLine()
                    {
                        Family = { FamilyMode = element.Key },
                        Step = _settings.GeneralSettings.Step,
                        Offset = _settings.GeneralSettings.Offset,
                    };

                    if (!_settings.DocLineSettings.TryGetValue(documentGuid, out var lineList))
                    {
                        lineList = new List<ReviLine>();
                        _settings.DocLineSettings[documentGuid] = lineList;
                    }
                    var currLine = lineList.FirstOrDefault(line => line.Family.FamilyMode == element.Key);
                    if (currLine == null)
                    {
                        lineList.Add(_line);
                    } else
                    {

                    }
                        docElementsInfo.Color = ColorMapper.FromSymbolColor(currLine.Color);
                    CableSystems.Add(docElementsInfo);
                    ReviMaxLog.Information($"Adding new line {_line.ToString()} to list. Now it's: {_settings.ToString()}");
                    Systems.Add(
                        new CableSystemRowVM(dispatcher)
                        {
                            Title = element.Key.GetDescription(),
                            SubTitle = $"Элементов: {element.Value.Count.ToString()}",
                            Settings = _settings,
                            LineSettings = lineList.FirstOrDefault(line=> line.Family.FamilyMode == element.Key),
                            Doc = _doc
                        }
                        );
                }
            }
        }
    }
}
