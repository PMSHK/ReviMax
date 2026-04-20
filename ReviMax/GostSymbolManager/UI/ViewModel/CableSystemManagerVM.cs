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
using ReviMax.Core.Config;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Converter;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Filters;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Providers.Factory;
using ReviMax.GostSymbolManager.Services;
using ReviMax.GostSymbolManager.Services.Utils;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.Revit.Config.Storage;
using ReviMax.Revit.Config.Storage.Model;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Bridge.Event;
using ReviMax.Revit.Core.Services;

namespace ReviMax.GostSymbolManager.UI.ViewModel
{
    public class CableSystemManagerVM : ViewModelBase
    {
        private readonly Document _doc;
        private CableSystemSettings _settings;
        private CableSystemFilterMode _currentFilterMode;
        private CableSystemService _cableSystemService;
        public ObservableCollection<CableSystemRowVM> Systems { get; } = new();

        public RevitDispatcherService dispatcher = new();
        public Dictionary<FamilyMode,IList<Element>> SelectedElements { get; set; }
        internal ICableSystemCategory CurrentFilter {  get; set; }
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
            
            if (_currentFilterMode.Equals(CableSystemFilterMode.SELECTED))
            {
                dispatcher.Request(
                request: app => { _cableSystemService.DrawCableSystemSymbols(CurrentFilter, _settings, SelectedElements);
                },
                callback: () => { ReviMaxLog.Information("Placing GOST symbols..."); }
                );
                return;
            }

            var currentFilter = GetCurrentFilter();
            if (currentFilter == null) return;
            dispatcher.Request(
                request: app => { _cableSystemService.DrawCableSystemSymbols(GetCurrentFilter()!, _settings);
                },
                callback: () => { ReviMaxLog.Information("Placing GOST symbols..."); }
                );
        }
        public void RedrawGostSymbols() 
        {
            ReviMaxLog.Information("Redrawing GOST symbols...");
            dispatcher.Request(
                request: app => { 
                    var groupedStroredInfo = ReviMaxStorage.GetInstanceByActiveView(Doc.ActiveView);

                    if (groupedStroredInfo.Count <= 0) return;

                    _cableSystemService.RedrawCableSystems(groupedStroredInfo, _settings);
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
            return _currentFilterMode switch
            {
                CableSystemFilterMode.ALL => new AllCableFilter() ,
                CableSystemFilterMode.TRAYS => new CableTrayFilter(),
                CableSystemFilterMode.CONDUITS => new ConduitFilter(),
                CableSystemFilterMode.SELECTED => CreateFilterForSelected(),
                _ => throw new InvalidOperationException("Invalid filter mode")
            };
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

                    LoadCableSystemRow(groupedElements);
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
                    var currentFilter = GetCurrentFilter();
                    if (currentFilter == null) return;
                    var elements = _cableSystemService.GetCableSystemsByCategory(currentFilter);

                    LoadCableSystemRow(elements);
                },
                callback: () =>
                {
                    ReviMaxLog.Information($"Manage data types for filter mode: {_currentFilterMode}");
                });
        }

        private void LoadCableSystemRow(Dictionary<FamilyMode, IList<Element>> elements)
        {
            Systems.Clear();
            foreach (var element in elements)
            {
                var _line = new ReviLine();
                _line.Family.FamilyMode = element.Key;
                _line.Step = _settings.GeneralSettings.Step;
                _line.Offset = _settings.GeneralSettings.Offset;
                var currLine = _settings.LineSettings.FirstOrDefault(s => s.Family.FamilyMode == element.Key);
                if (currLine == null) 
                {
                    _settings.LineSettings.Add(_line);
                }

                Systems.Add(
                    new CableSystemRowVM(dispatcher)
                    {
                        Title = element.Key.GetDescription(),
                        SubTitle = $"Элементов: {element.Value.Count.ToString()}",
                        Settings = _settings,
                        LineSettings = _settings.LineSettings.Where(ls => ls.Family.FamilyMode.Equals(element.Key)).FirstOrDefault(),
                        Doc = _doc
                    }
                    );
            }
        }
    }
}
