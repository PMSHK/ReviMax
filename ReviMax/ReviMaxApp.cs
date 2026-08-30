using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using ReviMax.Core.Config;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Parameters;


namespace ReviMax
{
    public class ReviMaxApp : IExternalApplication
    {
        private static string iconsPath;
        public Result OnStartup(UIControlledApplication application)
        {
            
            PathManager.Initialize();
            ReviMaxLog.Init();
            
            application.ControlledApplication.DocumentOpened += OnDocumentOpened;

            ImageLoader imageLoader = new ImageLoader();
            ReviMaxLog.Information("ReviMaxApp OnStartup started.");
            try
            {
                iconsPath = PathManager.GetIconsPath();

                string tabName = "ReviMax";
                application.CreateRibbonTab(tabName);
                var panel = application.CreateRibbonPanel(tabName, "ReviMax Panel");

                var SplitButtonData = new SplitButtonData("ReviMaxSplitBtn", "ReviMax Curve \n Tools");
                var splitButton = panel.AddItem(SplitButtonData) as SplitButton;

                var cableSystemWindow = new PushButtonData("CableSystemWindowBtn", "Cable \nSystem Manager",
                    System.Reflection.Assembly.GetExecutingAssembly().Location, "ReviMax.Commands.OpenCableSystemWindow");

                splitButton?.AddPushButton(cableSystemWindow);

                var numeratorButton = RevitButtonManager.CreateSmallPushButton("numeratorButton"
                    , "Нумератор"
                    , "ReviMax.Commands.OpenNumeratorWindow"
                    , "Помогает нумеровать элементы в зависимости от их расположения"
                    );
                var utilsPanel = application.CreateRibbonPanel(tabName, "Utils");
                utilsPanel.AddItem(numeratorButton);

                var AnalyzerButton = RevitButtonManager.CreateSmallPushButton("analyzerButton"
                   , "Анализатор"
                   , "ReviMax.Commands.OpenCableSystemAnalyzerWindow"
                   , "Позволяет построить граф системы кабелей, а также проставить метки, посчитать загруженость трасс и построить кабельный журнал"
                   );
                utilsPanel.AddItem(AnalyzerButton);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error("Error during OnStartup", ex);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void OnDocumentOpened(Object sender, DocumentOpenedEventArgs e)
        {
            Document doc = e.Document;
            if (doc != null)
            {
                ParameterBinder.EnsureSharedParameters(doc);
            }
        }
    }
}
