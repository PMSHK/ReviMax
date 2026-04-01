using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using ReviMax.Config;
using ReviMax.core.Elements;
using ReviMax.Core.Elements.Filters;
using ReviMax.Core.Elements.Providers.Factory;
using ReviMax.Core.Revit;
using ReviMax.Core.Utils.Converter;
using ReviMax.Core.Utils.Managers;
using ReviMax.Models.Graph;
using ReviMax.Models.Graph.Filter;
using ReviMax.Models.Revit;
using ReviMax.Services;
using ReviMax.Services.Utils;
using ReviMax.UI.Windows;

namespace ReviMax.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class OpenCableSystemWindow : IExternalCommand
    {

        public OpenCableSystemWindow() { }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            View _activeView;
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document Doc = uIDocument.Document;

            var window = new ReviMaxCableManagerWindow(Doc);
            //window.ShowDialog();
            window.Show();
            window.Activate();
            window.Focus();

            //TaskDialog.Show("ReviMax", "Не выбраны категории ReviMax элементов.");
            return Result.Succeeded;
        }
    }
}
