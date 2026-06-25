using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Dynamitey;
using ReviMax.Core.Config;
using ReviMax.Handlers;
using ReviMax.Revit.Model;

namespace ReviMax.Revit.Core.Services
{
    public class RevitElementsManager
    {

        public static void ShowElementsOnView(List<ElementId> elements, View activeView)
        {
            if (elements == null || elements.Count == 0) return;
            if (activeView == null) return;

            activeView.UnhideElements(elements);

            ReviMaxLog.Information($"Showing {elements.Count} elements on view {activeView.Name}");
        }

        public static void HideElementsOnView(List<ElementId> elements, View activeView)
        {
            if (elements == null || elements.Count == 0) return;
            if (activeView == null) return;
            
            activeView.HideElements(elements);

            ReviMaxLog.Information($"Hiding {elements.Count} elements on view {activeView.Name}");
        }

        internal static ResultHandler<List<XYZ>> GetCableSystemElementEndpoints(Element element) 
        {
            if (element == null) return ResultHandler<List<XYZ>>.Failure("Endpoints for element was not found");

            if (element.GetType() == typeof(MEPSystem))
            {
                var system = (MEPSystem)element;
                var connectors = system.ConnectorManager.Connectors;
                if (connectors.Size < 2) return ResultHandler<List<XYZ>>.Failure("Endpoints for element was not found");
                List<XYZ> endpoints = new List<XYZ>();
                foreach (Connector connector in connectors)
                {
                    if (connector!=null 
                        && connector.ConnectorType == ConnectorType.End
                        && connector.Domain == Domain.DomainCableTrayConduit)
                    {
                        endpoints.Add(connector.Origin);
                    }
                }
                return ResultHandler<List<XYZ>>.Success(endpoints);
            }

            return ResultHandler<List<XYZ>>.Failure("Endpoints for element was not found");

        }

        public ResultHandler<FilteredElementCollector> BuildCollector (ElementQuery query)
        {
            if (query == null) return ResultHandler<FilteredElementCollector>.Failure("Query is null");
            
                FilteredElementCollector collector =
            query.View == null
            ? new FilteredElementCollector(query.Doc)
            : new FilteredElementCollector(query.Doc, query.View.Id);
            return ResultHandler<FilteredElementCollector>.Success(collector);
        }

        public static Element? GetElementById(Document doc, ElementId elementId)
        {
            if (doc == null) return null;
            try
            {
                return doc.GetElement(elementId);
            }
            catch
            {
                return null;
            }
        }

        public ResultHandler<IEnumerable<Element>> GetElements (ElementQuery query, FilteredElementCollector collector)
        {
            if (query == null && collector == null) return ResultHandler<IEnumerable<Element>>.Failure("query or collector is null");
            

            List<ElementFilter> filters = new List<ElementFilter>();

            if (query.Categories.Count > 0)
            {
                filters.Add(new ElementMulticategoryFilter(query.Categories));
            }


            if (query.CustomFilters.Count > 0)
            {
                filters.AddRange(query.CustomFilters);
            }

            if (filters.Count > 0)
            {
                collector.WherePasses(new LogicalAndFilter(filters)).WhereElementIsNotElementType();
            }

            return ResultHandler<IEnumerable<Element>>.Success(collector.ToElements().ToList());
        }
        public static bool HasPlaced(Element element)
        {
            if (element == null) return false;
            return element.Location != null || element.get_BoundingBox(null) != null; ;
        }

        public static bool IsNestedSubComponent(Element e)
        {
            return e is FamilyInstance fi && fi.SuperComponent != null;
        }

        public static string SafeName(Element? e)
        {
            try
            {
                if (e == null || !e.IsValidObject) return "<invalid>";
                return e.Name ?? "<no name>";
            }
            catch
            {
                return "<name error>";
            }
        }

    }
}
