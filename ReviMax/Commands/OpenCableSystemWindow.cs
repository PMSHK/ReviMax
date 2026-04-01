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
using ReviMax.Core.Config;
using ReviMax.Core.Utils.Converter;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.Models.Graph;
using ReviMax.GostSymbolManager.Models.Graph.Filter;
using ReviMax.GostSymbolManager.Models.Revit;
using ReviMax.GostSymbolManager.Services;
using ReviMax.GostSymbolManager.Services.Utils;
using ReviMax.GostSymbolManager.UI.Windows;

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
