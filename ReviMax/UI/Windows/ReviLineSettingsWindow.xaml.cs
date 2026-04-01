using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using ReviMax.Models.Annotations;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Bridge.Event;
using ReviMax.UI.ViewModel;

namespace ReviMax.UI.Windows
{
    /// <summary>
    /// Interaction logic for ReviLineSettingsWindow.xaml
    /// </summary>
    public partial class ReviLineSettingsWindow : Window
    {
        internal ReviLineSettingsWindow(Document doc, CableSystemSettings settings, ReviLine lineSettings, RevitDispatcherService dispatcher)
        {
            InitializeComponent();
            var viewModel = new ReviLineSettingsVM(doc, settings, lineSettings, dispatcher);
            DataContext = viewModel;
            viewModel.RequestClose += () => this.Close();
        }
    }
}
