using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;

//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using ReviMax.Core.Config;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.Revit.Parameters;


namespace ReviMax
{
    public class ReviMaxApp : IExternalApplication
    {
        private static string iconsPath;
        //private static ILogger logger;
        public Result OnStartup(UIControlledApplication application)
        {
            //logger = MicrosoftLogger.CreateLogger<ReviMaxApp>();
            
            PathManager.Initialize();
            ReviMaxLog.Init();
            
            application.ControlledApplication.DocumentOpened += OnDocumentOpened;

            ImageLoader imageLoader = new ImageLoader();
            ReviMaxLog.Information("ReviMaxApp OnStartup started.");
            try
            {
                //iconsPath = Path.Combine(
                //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                //    "Icons");
                iconsPath = PathManager.GetIconsPath();

                string tabName = "ReviMax";
                application.CreateRibbonTab(tabName);
                var panel = application.CreateRibbonPanel(tabName, "ReviMax Panel");

                var SplitButtonData = new SplitButtonData("ReviMaxSplitBtn", "ReviMax Curve \n Tools");
                var splitButton = panel.AddItem(SplitButtonData) as SplitButton;


                var buttonData = new PushButtonData("CableSystemSymbolManagerBtn", "Run Cable \nSystem Manager",
                    System.Reflection.Assembly.GetExecutingAssembly().Location, "ReviMax.Commands.CableSystemSymbolCommand");

                string filePath = PathManager.GetFilePathInDirectory(PathManager.GetIconsPath(), "revit.png");
                BitmapImage image = imageLoader.Load(filePath);
                ReviMaxLog.Information($"Loaded image for button {image!=null}");
                if (image != null) {
                    buttonData.LargeImage = image; 
                    buttonData.Image = image;}

                var deleteButton = new PushButtonData("DeleteSymbolsBtn", "Delete Symbols",
                    System.Reflection.Assembly.GetExecutingAssembly().Location, "ReviMax.Commands.DeleteCableSystemSymbolCommand");

                var cableSystemWindow = new PushButtonData("CableSystemWindowBtn", "Cable \nSystem Manager",
                    System.Reflection.Assembly.GetExecutingAssembly().Location, "ReviMax.Commands.OpenCableSystemWindow");

                splitButton?.AddPushButton(buttonData);
                splitButton?.AddPushButton(deleteButton);
                splitButton?.AddPushButton(cableSystemWindow);
                //panel.AddStackedItems(buttonData, deleteButton);
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
            // Код очистки ресурсов при закрытии Revit
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
