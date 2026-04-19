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
using ReviMax.NumeratorManager.UI.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReviMax.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class OpenNumeratorWindow : IExternalCommand
    {

        public OpenNumeratorWindow() { }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var assemblyName = new AssemblyName(args.Name).Name + ".dll";
                var pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var assemblyPath = Path.Combine(pluginFolder, assemblyName);
                ReviMaxLog.Information($"Assembly path: {assemblyPath}");
                return File.Exists(assemblyPath) ? Assembly.Load(assemblyPath) : null;
            };

            SortedDictionary<string,string> d = new SortedDictionary<string,string>();

            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document Doc = uIDocument.Document;
            try { 
            var window = new NumeratorWindow(Doc);
            window.Show();
            window.Activate();
            window.Focus();
            return Result.Succeeded;
            } catch (System.Windows.Markup.XamlParseException xamlEx)
            {
                var inner = xamlEx.InnerException ?? xamlEx;
                ReviMaxLog.Error($"REAL ERROR: {inner.GetType().Name} | {inner.Message}");
                TaskDialog.Show("Ошибка", $"Inner: {inner.Message}\nStack: {inner.StackTrace}");
                return Result.Failed;
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error(ex.Message);
                return Result.Failed;
            }
        }
    }
}
