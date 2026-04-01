using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ReviMax.Core.Utils.Converter;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Bridge.Event;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.Revit.Core.Services;
using ReviMax.Core.Config;
namespace ReviMax.GostSymbolManager.UI.ViewModel
{
    public class FamilySettingsVM : INotifyPropertyChanged
    {
        private bool isBusy = false;

        private Document _doc;
        private ReviLine _lineSettings;
        private FamilyService _familyService;

        public Document Doc => _doc;
        public ReviLine LineSettings => _lineSettings;

        public event Action CloseRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        private RevitDispatcherService _dispatcher;
        public ReviLine DraftLineSettings { get; set; } = new ReviLine();

        public ICommand Accept { get; }
        public ICommand Decline { get; }


        private double _stepValue;
        private double _offsetValue;
        private double _glyphValue;

        public string StepValue
        {
            get => _stepValue.ToString(); 
            set
            {
                ReviMaxLog.Information($"StepValue setter called with value: {value}");
                if (!string.IsNullOrWhiteSpace(_stepValue.ToString()) && _stepValue.ToString() != value)
                {
                    if (_stepValue.ToString() != value)
                    {
                        double.TryParse(value, out double result);
                        _stepValue = result;
                        OnPropertyChanged(nameof(StepValue));
                    }
                }
            }
        }
        public string OffsetValue
        {
            get => _offsetValue.ToString(); 
            set
            {
                ReviMaxLog.Information($"OffsetValue setter called with value: {value}");
                if (!string.IsNullOrWhiteSpace(_offsetValue.ToString()) && _offsetValue.ToString() != value)
                {
                    if (_offsetValue.ToString() != value)
                    {
                        double.TryParse(value, out double result);
                        _offsetValue = result;
                        OnPropertyChanged(nameof(OffsetValue));
                    }
                }
            }
        }

        public string GlyphSize 
        {
            get => _glyphValue.ToString();
            set 
            {
                ReviMaxLog.Information($"GlyphSizeValue setter called with value: {value}");
                if (!string.IsNullOrWhiteSpace(_glyphValue.ToString()) && _glyphValue.ToString() != value)
                {
                    if (_glyphValue.ToString() != value)
                    {
                        double.TryParse(value, out double result);
                        _glyphValue = result;
                        OnPropertyChanged(nameof(GlyphSize));
                    }
                }
            }
        }

        private ObservableCollection<FamilyVM> _familySet = new ();
        public ObservableCollection<FamilyVM> FamilySet {
            get { return _familySet; }
            set { _familySet = value;
                OnPropertyChanged(nameof(FamilySet));
            }
        }

        private FamilyVM _selectedFamily;
        public FamilyVM SelectedFamily { 
            get { return _selectedFamily; } 
            set {
                if (_selectedFamily != value)
                {
                    _selectedFamily = value;
                    
                    
                    if (_selectedFamily != null) 
                    {
                        ReviMaxLog.Information($"Selected family changed to: {value?.Title}");
                        _selectedFamily = value;
                    }
                    OnPropertyChanged(nameof(SelectedFamily));
                }
                
            } 
        }



        public FamilySettingsVM(Document doc, ReviLine settings, RevitDispatcherService dispatcher)
        {
            _dispatcher = dispatcher;
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
            _lineSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            DraftLineSettings = (ReviLine)settings.Clone();
            Accept = new ReviMaxCommand(OnAccept,CanAcceptChanges);
            Decline = new ReviMaxCommand(OnDecline);
            StepValue = DraftLineSettings.Step.FeetToMillimeters().ToString();
            OffsetValue = DraftLineSettings.Offset.FeetToMillimeters().ToString();
            GlyphSize = DraftLineSettings.GlyphSize.FeetToMillimeters().ToString();

            _familyService = new FamilyService(Doc);
            var familyLib = _familyService.FindAll();
            FamilySet.Clear();

            foreach (var family in familyLib.Select(f => f.Value))
            {
                FamilySet.Add(family);
            };
            SelectedFamily = FamilySet.FirstOrDefault(f => f.Title.Equals(DraftLineSettings.Family.Family.FamilyName, StringComparison.InvariantCultureIgnoreCase));
            IfFamiliesLoaded();
        }

        private void IfFamiliesLoaded() 
        {
            Dictionary<string, bool> loadedMap = null;
            _dispatcher.Request(
                request: app => 
                {
                    loadedMap = FamilySet.ToDictionary(
                        f => f.Title,
                        f => _familyService.IsFamilyLoaded(f.Title));
                },
                callback: () => 
                {
                    if (loadedMap == null) return;
                    foreach (var family in FamilySet) 
                    {
                        if (loadedMap.TryGetValue(family.Title, out bool isLoaded))
                            family.IsLoaded = isLoaded;
                    }
                });

        }



        private void OnAccept()
        {
            if (isBusy) return;
            isBusy = true;
            LineSettings.CoppyFrom(DraftLineSettings);
            if (double.TryParse(StepValue, out double step))
                LineSettings.Step = step.MillimetersToFeet();

            if (double.TryParse(OffsetValue, out double offset))
                LineSettings.Offset = offset.MillimetersToFeet();

            if (double.TryParse(GlyphSize, out double glyphSize))
                LineSettings.GlyphSize = glyphSize.MillimetersToFeet();
            ReviMaxLog.Information($"Accepting changes: Step={LineSettings.Step}, Offset={LineSettings.Offset}, GlyphSize={LineSettings.GlyphSize}");
            if (SelectedFamily != null)
            {

                var title = _selectedFamily.Title;
                _dispatcher.Request(
                    request: app => { 
                        _familyService.LoadFamily(title);
                        _familyService.FillFamilyProperties(SelectedFamily.Title, LineSettings.Family.Family);
                    },
                    callback: () =>
                    {
                        
                        ReviMaxLog.Information($"Load family: {title}");
                        CloseRequested?.Invoke();
                        isBusy = false;
                    }
                    );
                return;
            }

            CloseRequested?.Invoke();
        }

        private void OnDecline()
        {
            CloseRequested?.Invoke();
        }

        private bool CanAcceptChanges()
        {
            return double.TryParse(StepValue, out _) && double.TryParse(OffsetValue, out _) && double.TryParse(GlyphSize, out _) && !isBusy;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
