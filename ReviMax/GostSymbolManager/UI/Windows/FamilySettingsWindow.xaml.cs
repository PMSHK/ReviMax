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
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Bridge.Event;
using ReviMax.GostSymbolManager.UI.ViewModel;
using ReviMax.GostSymbolManager.Models.Annotations;

namespace ReviMax.UI.Windows
{
    /// <summary>
    /// Interaction logic for FamilySettingsWindow.xaml
    /// </summary>
    public partial class FamilySettingsWindow : Window
    {
        public FamilySettingsWindow(Document doc, ReviLine settings, RevitDispatcherService dispatcher)
        {
            InitializeComponent();
            var viewModel = new FamilySettingsVM(doc, settings, dispatcher);
            DataContext = viewModel;

            //viewModel.Init();

            viewModel.CloseRequested += () => this.Close();
        }
    }
}
