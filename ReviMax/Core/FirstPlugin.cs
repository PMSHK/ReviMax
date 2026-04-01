using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace ReviMax.core
{
    [Transaction(TransactionMode.Manual)]
    internal class FirstPlugin : IExternalCommand
    {
            public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
            {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            Reference myRef = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Выберите элемент");
            ElementId id = myRef.ElementId;
            TaskDialog.Show("Revit Plugin", $"Вы выбрали элемент с ID: {id?.ToString()}");
            TaskDialog.Show("Revit Plugin", "Привет! Плагин успешно запущен.");
                return Result.Succeeded;
            }
    }
}
