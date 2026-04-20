using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using ReviMax.Core.Comparator;
using ReviMax.Core.Config;
using ReviMax.Core.Enums;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.GostSymbolManager.UI.ViewModel;
using ReviMax.NumeratorManager.Services;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Services;
using ReviMax.UI.ViewModel;

namespace ReviMax.NumeratorManager.UI.ViewModel
{
     class NumeratorWindowViewModel : ViewModelBase
    {
        private readonly Document _doc;
        public RevitDispatcherService dispatcher = new();
        private CoordinateDirection _currentOrder;
        public CoordinateDirection CurrentOrder 
        {
            get => _currentOrder;
            set 
            {
                if (_currentOrder != value)
                {
                    ReviMaxLog.Information($"Order changed to: {_currentOrder}");
                    _currentOrder = value;
                    XYZCompareService.Direction = _currentOrder;
                    Comparer = XYZCompareService.ElementComparer;
                    if(Elements!= null && Elements.Length > 0) Array.Sort(Elements, Comparer);

                    OnPropertyChanged(nameof(CurrentOrder));
                }
            }
        }

        private SelectedParameter _selectedParameter;

        private ObservableCollection<SelectedParameter> _parameters = new();
        public SelectedParameter? SelectedParameter
        {
            get => _selectedParameter;
            set
            {
                if (_selectedParameter != value)
                {
                    _selectedParameter = value;
                    ReviMaxLog.Information($"Selected parameter changed to: {value?.Name}");
                    if (_selectedParameter != null && !string.Equals(_selectedParameter?.Name, value?.Name, StringComparison.Ordinal))
                    {
                        if (value != null)
                        {
                            _selectedParameter = value;
                        }
                    }

                    OnPropertyChanged(nameof(SelectedParameter));
                }
            }
        }
        public ObservableCollection<SelectedParameter> Parameters
        {
            get => _parameters;
            set
            {
                if (value != null)
                {
                    _parameters = value;
                    ReviMaxLog.Information($"parameters Changed {value.ToString()}");
                    OnPropertyChanged(nameof(Parameters));
                }
            }
        }


        public Element[] Elements { get; set; } 
        public HashSet<string> GeneralParameters { get; set; } = new();

        public ICommand SelectElements {  get; }
        public ICommand RenameElements {  get; }

        public String GeneralText { get; set; } = String.Empty;
        public String Prefix { get; set; } = String.Empty;
        public String Suffix { get; set; } = String.Empty;
        public String Separator { get; set; } = String.Empty;
        public String Step { get; set; } = String.Empty;
        public string StartValue {  get; set; } = String.Empty;
        public Comparison<Element> _comparer;
        public Comparison<Element> Comparer { get => _comparer!=null ? _comparer : (a,b)=>1;
            set {
            if (_comparer != value)
            {
                _comparer = value;
                OnPropertyChanged(nameof(Comparer));
            }
            } }
        

        public NumeratorWindowViewModel(Document doc)
        {
            _doc = doc;

            SelectElements = new ReviMaxCommand(Select,()=>true);
            RenameElements = new ReviMaxCommand(Rename, ()=> true);

            CurrentOrder = CoordinateDirection.X;
            XYZCompareService.Direction = _currentOrder;
            Comparer = XYZCompareService.ElementComparer;

            OnPropertyChanged(nameof(CurrentOrder));
        }

        public void Select()
        {
            Elements = [];
            GeneralParameters.Clear();
            Parameters.Clear();

            var service = new RevitSelectionService(new UIDocument(_doc));
            var elements = service.PickSameTypeByRectangle();

            ReviMaxLog.Information($"Elements not null {elements!=null}. Loaded {elements?.Count} elements");
            if (elements == null || elements.Count == 0) return;
            Elements = elements.ToArray();
            Array.Sort(Elements,Comparer);
            var parameterService = new RevitParametersManager();
            var firstElementParameters = parameterService.GetAllEditableParameters(elements[0]);
            foreach (var parameter in firstElementParameters) 
            {
                GeneralParameters.Add(parameter);
            }
            foreach (var element in elements)
            {
                ReviMaxLog.Information($"element is: {element.Name}");
                List<string> parameters = parameterService.GetAllEditableParameters(element);
                GeneralParameters.IntersectWith(parameters);
                ReviMaxLog.Information($"Got {parameters.Count} parameters from {element.Name}");
            }
            
            foreach(var parameterName in GeneralParameters)
            {
                if(parameterName == null) continue;
                Parameters.Add(new SelectedParameter(parameterName));
            }
        }

        public void Rename()
        {
            if(Elements == null || Elements.Length == 0 || SelectedParameter == null ||string.IsNullOrWhiteSpace(SelectedParameter?.Name)) return;
            ReviMaxLog.Information($"Elements for renaming: {Elements.Length}");
            int currNum = 1;
            if (!string.IsNullOrWhiteSpace(StartValue)) 
            {
                if (int.TryParse(StartValue, out int value)) 
                {
                    currNum = value;
                }
            }
            
            var parameterService = new RevitParametersManager();

            dispatcher.Request(
                    request: app => {
                        TransactionManager.StartTransaction(_doc, "Renumbering",
                    (doc) =>
                    {
                        foreach (var element in Elements)
                        {
                            if (SelectedParameter == null) continue;
                            bool isStepParsed = int.TryParse(Step, out int step);
                            var currValue = parameterService.LookUpParameterValue(element, SelectedParameter.Name);
                            var newValue = ParameterTextService.BuiltText
                                (string.IsNullOrWhiteSpace(GeneralText) ? currValue : GeneralText
                                , Prefix
                                , Suffix
                                , Separator
                                , string.IsNullOrEmpty(Step) ? null : isStepParsed ? currNum : (int?)null,
                                string.IsNullOrWhiteSpace(GeneralText) ? false : true ); 
                            currNum += step;
                            parameterService.SetFamilyParameter(element, SelectedParameter.Name, newValue);
                        }
                    });

                    },
                    callback: () => ReviMaxLog.Information($"Renumbered  {Elements.Length} elements")); 
        }
    }
}
