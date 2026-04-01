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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using ReviMax.UI.ViewModel;

namespace ReviMax.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для CableSystemManagerWindow.xaml
    /// </summary>
    public partial class CableSystemManagerUserControl : UserControl
    {
        public CableSystemManagerUserControl(Document doc)
        {
            try
            {
                InitializeComponent();
                DataContext = new CableSystemManagerVM(doc);
            }
            catch (Exception ex)
            {
                Autodesk.Revit.UI.TaskDialog.Show("ReviMax UI error", ex.ToString());
                throw;
            }
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.DispatcherUnhandledException += (s, e) =>
                {
                    Autodesk.Revit.UI.TaskDialog.Show("WPF error", e.Exception.ToString());
                    e.Handled = true;
                };
            }

        }
    }
}
