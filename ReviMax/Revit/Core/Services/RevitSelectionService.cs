using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ReviMax.Revit.Core.Services
{
    internal class RevitSelectionService
    {
        private UIDocument _uiDoc;

        public RevitSelectionService(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
        }

        public List<Element> GetSelectionCableSystems() 
        {
            IList<Reference> refs = _uiDoc.Selection.PickObjects(
        Autodesk.Revit.UI.Selection.ObjectType.Element,
        "Выберите лотки и/или трубы");

            var selectedElements = refs
                .Select(r => _uiDoc.Document.GetElement(r))
                .Where(e => e != null)
                .Where(e => e.Category != null)
                .Where(e =>
                {
                    int catId = e.Category.Id.IntegerValue;
                    return catId == (int)BuiltInCategory.OST_CableTray
                        || catId == (int)BuiltInCategory.OST_Conduit
                        || catId == (int)BuiltInCategory.OST_CableTrayFitting
                        || catId == (int)BuiltInCategory.OST_ConduitFitting;
                })
                .ToList();
            return selectedElements??[];
        }

        public Element? GetSelectionCableSystem()
        {
            Reference reference = _uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Выберите элемент \"Лоток\" или \"Труба\"");
            if (reference != null ) 
            {
                Element element = _uiDoc.Document.GetElement(reference);
                if(element != null && element.Category != null)
                {
                    int catId = element.Category.Id.IntegerValue;
                    if (catId == (int)BuiltInCategory.OST_CableTray
                        || catId == (int)BuiltInCategory.OST_Conduit
                        || catId == (int)BuiltInCategory.OST_CableTrayFitting
                        || catId == (int)BuiltInCategory.OST_ConduitFitting)
                    {
                        return element;
                    }
                }
            }
            return null;
        }


    }
}
