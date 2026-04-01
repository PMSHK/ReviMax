using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
using ReviMax.Models.Graph;
using ReviMax.Models.Graph.Filter;
using ReviMax.Models.Revit;
using ReviMax.Services;
using ReviMax.Services.Utils;

namespace ReviMax.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class DeleteCableSystemSymbolCommand : IExternalCommand
    {

        public DeleteCableSystemSymbolCommand() { }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            View _activeView;
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document Doc = uIDocument.Document;
            View activeView = Doc.GetActiveView();
            CleanupManager cleanupManager = new(Doc);
            Reference myRef = uIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Выберите элемент для удаления всей группы");
            ElementId elementId = myRef.ElementId;
            
            var selectedElement = Doc?.GetElement(elementId);
            if (selectedElement != null)
            {
                string runId = ReviMaxStorage.GetRunId(selectedElement);
                if (runId != null)
                {
                    int num = cleanupManager.DeleteReviMaxElement(runId, activeView.Id);
                    ReviMaxLog.Information($"Удалено {num} символов кабельных систем с RunId: {runId} на виде {activeView.Name}");
                    TaskDialog.Show("ReviMax", "Команда удаления символов успешно выполнена.");
                    return Result.Succeeded;
                }
            }

            TaskDialog.Show("ReviMax", "Не выбраны категории ReviMax элементов.");
            return Result.Succeeded;
        }
    }
}
