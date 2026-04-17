using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.GostSymbolManager.UI.ViewModel;
using ReviMax.Revit.Core.Services;
using ReviMax.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace ReviMax.NumeratorManager.UI.ViewModel
{
    class NumeratorWindowViewModel : ViewModelBase
    {
        private readonly Document _doc;
        private string _currentOrder;
        public string CurrentOrder 
        {
            get => _currentOrder;
            set 
            {
                if (value != null)
                {
                    ReviMaxLog.Information($"Order changed to: {value}");
                    _currentOrder = value;
                    OnPropertyChanged(nameof(CurrentOrder));
                }
            }
        }

        private ObservableCollection<SelectedParameter> _parameters = new();
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



        public List<Element> Elements { get; set; } = new();
        public SortedSet<string> GeneralParameters { get; set; } = new();

        public ICommand SelectElements {  get; }
        public ICommand RenameElements {  get; }

        public String GeneralText { get; set; } = String.Empty;
        public String Prefix { get; set; } = String.Empty;
        public String Suffix { get; set; } = String.Empty;
        public String Separator { get; set; } = String.Empty;
        public String Step { get; set; } = String.Empty;

        public NumeratorWindowViewModel(Document doc)
        {
            _doc = doc;

            SelectElements = new ReviMaxCommand(Select,()=>true);
            RenameElements = new ReviMaxCommand(Rename, ()=>true);
        }

        public void Select()
        {
            Elements.Clear();
            GeneralParameters.Clear();
            Parameters.Clear();
            var service = new RevitSelectionService(new UIDocument(_doc));
            var elements = service.GetSelectedElements();
            if (elements == null || elements.Count == 0) return;
            Elements.AddRange(elements);
            var parameterService = new RevitParametersManager();
            foreach (var element in elements)
            {
                var instance = element as FamilyInstance;
                if (instance == null) continue;
                List<string> parameters = parameterService.GetAllParameters(instance);
                GeneralParameters.IntersectWith(parameters);
            }
            
            foreach(var parameterName in GeneralParameters)
            {
                Parameters.Add(new SelectedParameter(parameterName));
            }
        }

        public void Rename()
        {

        }
    }
}
