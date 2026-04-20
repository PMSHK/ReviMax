using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Dynamitey.DynamicObjects;
using ReviMax.Core.Config;
using ReviMax.Revit.Core.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var list = GetElementsSafely<List<Element>>(() =>
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

            });

            return list ?? new List<Element>();
        }

        public Element? GetSelectionCableSystem()
        {
            var element = GetElementsSafely<Element>(() =>
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
            });
            return element ?? null;
        }

        public IList<Element>? GetSelectedElements()
        {
            var list = GetElementsSafely<IList<Element>>(() =>
            {
                var selectedElements = _uiDoc.Selection.PickElementsByRectangle();
                ReviMaxLog.Information($"Selected: {selectedElements.Count} elements");
                return selectedElements;
            });

            return list != null ? list : new List<Element>();
        }

        public IList<Element> PickSameTypeByRectangle()
        {
            var list = GetElementsSafely<IList<Element>>(() => 
            {
                Reference pickedRef = _uiDoc.Selection.PickObject(
                        ObjectType.Element,
                        "Выберите элемент-образец");

                if (pickedRef == null)
                    return new List<Element>();

                Document doc = _uiDoc.Document;
                Element sample = doc.GetElement(pickedRef);
                if (sample == null)
                    return new List<Element>();

                ElementId sampleTypeId = sample.GetTypeId();

                var filter = new SameTypeSelectionFilter(sampleTypeId);

                IList<Element> selected = _uiDoc.Selection.PickElementsByRectangle(
                    filter,
                    "Выделите рамкой элементы того же типа");

                ReviMaxLog.Information($"Sample element id={sample.Id}, typeId={sampleTypeId}");
                ReviMaxLog.Information($"Selected {selected.Count} elements of same type");

                return selected;
            });

            return list!=null? list : new List<Element>();
        }

        public IList<Element> GetSelectedElements(ISelectionFilter selectionFilter)
        {
            var list = GetElementsSafely<IList<Element>>(() => 
            {
                var selectedElements = _uiDoc.Selection.PickElementsByRectangle(selectionFilter);
                ReviMaxLog.Information($"Selected: {selectedElements.Count} elements");
                return selectedElements;
            });

            return list != null ? list : new List<Element>();
        }

        public T? GetElementsSafely<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                ReviMaxLog.Information("Selection was canceled by user");
                return default;
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Error withing selection: {ex.Message}");
                return default;
            }
        }

        }
    }
