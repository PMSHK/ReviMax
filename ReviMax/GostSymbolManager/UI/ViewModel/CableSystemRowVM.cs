using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Bridge.Event;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.UI.Windows;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.Core.Config;

namespace ReviMax.GostSymbolManager.UI.ViewModel
{
    public class CableSystemRowVM : ViewModelBase
    {
        public string Title { get; set; } = "";
        public string SubTitle { get; set; } = "";
        public ICommand Edit { get; }
        public Document Doc { get; set; }
        internal CableSystemSettings Settings { get; set; } = new();
        internal ReviLine LineSettings { get; set; } = new();
        private RevitDispatcherService _dispatcher;


        public CableSystemRowVM(RevitDispatcherService dispatcher)
        {
            _dispatcher = dispatcher;
            Edit = new ReviMaxCommand(EditLine, NotEmpty);
        }

        public void EditLine() 
        {
            ReviMaxLog.Information($"Pressed edit for {Title}");
            ReviMaxLog.Information($"Current settings: {Settings}, {LineSettings}");
            var window = new ReviLineSettingsWindow(Doc, Settings, LineSettings, _dispatcher);
            window.Show();
            window.Activate();
        }
        public bool NotEmpty()
        {
            return !(string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(SubTitle));
        }
    }
}
