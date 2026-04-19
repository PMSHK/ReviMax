using Autodesk.Revit.DB;
using ReviMax.NumeratorManager.UI.ViewModel;
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

namespace ReviMax.NumeratorManager.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для NumeratorWindow.xaml
    /// </summary>
    public partial class NumeratorWindow : Window
    {
        public NumeratorWindow(Document doc)
        {
            InitializeComponent();
            var ViewModel = new NumeratorWindowViewModel(doc);
            DataContext = ViewModel;
        }
    }
}
