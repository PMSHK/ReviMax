using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using ReviMax.Core.Config;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Converter;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Mapper;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Services;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.Revit.Config.Storage;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Bridge.Event;
using ReviMax.Revit.Core.Services;
using ReviMax.UI.Windows;

namespace ReviMax.GostSymbolManager.UI.ViewModel
{
    public class ReviLineSettingsVM : ViewModelBase
    {
        private bool isBusy;
        public event Action RequestClose;
        public Document Doc { get; }
        private RevitFilterManager filterManager;
        private ReviLineService lineService;
        public CableSystemSettings Settings { get; set; }

        private LineRowVM? _selectedLineStyle;
        public ReviLine LineSettings { get; set; }
        private ReviLine _tmpLine;

        public LineRowVM? SelectedLineStyle
        {
            get => _selectedLineStyle;
            set {
                if (_selectedLineStyle != value)
                {
                    _selectedLineStyle = value;
                    ReviMaxLog.Information($"Selected line style changed to: {SelectedLineStyle?.Name}, LineSettings name: {LineSettings.Name}, value: {value?.Name}");
                    ReviMaxLog.Information($"LineSettings is null: {LineSettings == null}, value is null: {value == null}, LineSettings {LineSettings?.ToString()}");
                    if (LineSettings != null && !string.Equals(LineSettings?.Name, value?.Name, StringComparison.Ordinal))
                    {
                        //var foudnLine = lineService.BuildReviLine(value?.Name, LineSettings.Family.FamilyMode);
                        //_tmpLine = foudnLine?.Clone() as ReviLine;
                        //_tmpLine?.Offset = LineSettings.Offset;
                        //_tmpLine?.Step = LineSettings.Step;
                        //_tmpLine?.GlyphSize = LineSettings.GlyphSize;
                        if (value != null) { 
                        UpdateReviLine(value);
                        }
                        ReviMaxLog.Information($"Built line from selected style: {_tmpLine?.Name}, CategoryId: {_tmpLine?.CategoryId}, Color: {_tmpLine?.Color}, Weight: {_tmpLine?.Weight}");

                    }

                        OnPropertyChanged(nameof(SelectedLineStyle));
                }
            } 

        }


        public ObservableCollection<LineRowVM> LineStyles { get; } = new();

        private List<Category> _categories;
        public List<Category> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value && value!=null)
                {
                    _categories = value;
                    OnPropertyChanged(nameof(Categories));
                }
            }
        }

        public ICommand Accept { get; }
        public ICommand Decline { get; }
        public ICommand LoadSettings { get; }
        public ICommand SaveSettings { get; }
        public ICommand EditLineSymbol { get; }
        private RevitDispatcherService _dispatcher;

        public ReviLineSettingsVM(Document doc, CableSystemSettings settings, RevitDispatcherService dispatcher)
        {
            _dispatcher = dispatcher;
            Doc = doc ?? throw new ArgumentNullException(nameof(doc));

            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            
            filterManager = new RevitFilterManager(Doc);


            lineService = new ReviLineService(Doc, filterManager?.GetLineStyles());

            FindLineStyles();
            
        }

        public ReviLineSettingsVM(Document doc, CableSystemSettings settings, ReviLine lineSettings, RevitDispatcherService dispatcher)
        {
            _dispatcher= dispatcher;
            Doc = doc ?? throw new ArgumentNullException(nameof(doc));
            
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            LineSettings = lineSettings ?? throw new ArgumentNullException(nameof(lineSettings));

                filterManager = new RevitFilterManager(Doc);

            ReviMaxLog.Information($"Initialized filter manager in {nameof(ReviLineSettingsVM)} constructor, filterManager is null: {filterManager == null}, and {filterManager?.GetLineStyles()[0]}");
            lineService = new ReviLineService(Doc, filterManager?.GetLineStyles());

            FindLineStyles();
            Accept = new ReviMaxCommand(AcceptChandes, CanAcceptChanges);
            Decline = new ReviMaxCommand(DeclineChanges);
            EditLineSymbol = new ReviMaxCommand(OnSymbolEdit);
            LoadSettings = new ReviMaxCommand(()=>
            {
                if (SaveLoadFileService.LoadFromFile<CableSystemSettingsDto>(PathManager.GetAppDataPath(), out var fileSettings))
                {
                    var fileName = Path.GetFileNameWithoutExtension(fileSettings.path);
                    Settings.CoppyFrom(fileSettings.data.ToModel());
                    var foundLineSettings = Settings.LineSettings.FirstOrDefault(ls => ls.Name == LineSettings.Name);
                    if (foundLineSettings != null) LineSettings.CoppyFrom(foundLineSettings);
                    _tmpLine = LineSettings.Clone() as ReviLine;
                    SelectedLineStyle = LineStyles.FirstOrDefault(s =>
                                        string.Equals(s.Name, LineSettings.Name, StringComparison.Ordinal));


                    var actual = Settings.LineSettings.FirstOrDefault(ls => ls.Name == LineSettings.Name);
                }
            }, () => true);
            SaveSettings = new ReviMaxCommand(() => SaveLoadFileService.SaveToFile<CableSystemSettingsDto>(Settings.ToDto()), () => Settings != null && Settings.Filled() );
            SelectedLineStyle = LineStyles.FirstOrDefault(s => string.Equals(s.Name, LineSettings.Name, StringComparison.Ordinal));

            _tmpLine = LineSettings?.Clone() as ReviLine;

            ReviMaxLog.Information($"Selected line style: {SelectedLineStyle?.Name}, LineSettings name: {LineSettings?.Name}, value: {SelectedLineStyle?.Name}, tmpLine: {_tmpLine?.ToString()}");
        }

        public void FindLineStyles()
        {
            LineStyles.Clear();
            Categories = lineService.LineStyles;
            if (Categories == null) return;

            foreach (var style in Categories)
            {
                LineStyles.Add(new LineRowVM()
                {
                    Name = style.Name,
                }
                );
            }
        }

        private void AcceptChandes()
        {
            if (_tmpLine == null)
            {
                CloseWindow();
                return;
            }
            ReviMaxLog.Information($"ReferenceEquals(LineSettings, _tmpLine): {ReferenceEquals(LineSettings, _tmpLine)}");
            
            if (SelectedLineStyle != null)
            {
                UpdateReviLine(SelectedLineStyle);
                LineSettings.CoppyFrom(_tmpLine);
                _dispatcher.Request(
                    request: (app) =>
                    {
                        TransactionManager.StartTransaction(Doc, "Update line settings", (d) => ReviMaxProjectSettingsStorage.Save(d, Settings.ToDto()));
                    });
            }
            ReviMaxLog.Information($"Accepting changes. LineSettings: {LineSettings?.ToString()}, _tmpLine: {_tmpLine?.ToString()}");

            CloseWindow();
        }

        private void DeclineChanges() 
        {
            if (isBusy) return;
            CloseWindow();
        }

        private bool CanAcceptChanges() 
        {
            return _tmpLine != null && !isBusy;
        }

        private void CloseWindow()
        {
            RequestClose?.Invoke();
        }

        public void OnSymbolEdit()
        {
            if (isBusy) return;
            isBusy = true;
            var window = new FamilySettingsWindow(Doc, LineSettings, _dispatcher);
            window.Closed += (s, e) => { isBusy = false; };
            window.Show();
            window.Activate();

        }

        private void UpdateReviLine(LineRowVM model)
        {   
            var foudnLine = lineService.BuildReviLine(model.Name, LineSettings.Family.FamilyMode);
            _tmpLine = foudnLine?.Clone() as ReviLine;
            _tmpLine?.Offset = LineSettings.Offset;
            _tmpLine?.Step = LineSettings.Step;
            _tmpLine?.GlyphSize = LineSettings.GlyphSize;
        }
    }
}
