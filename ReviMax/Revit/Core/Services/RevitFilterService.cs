using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ReviMax.GostSymbolManager.Filters;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Providers.Factory;

namespace ReviMax.Revit.Core.Services
{
    internal class RevitFilterService
    {
        public static FilterRule CreateFilterRule(ElementId parameterId,string value)
        {
            return ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value);
        }

        public static ParameterFilterElement GetFilter(Document doc, string filterName, List<ElementId> categories)
        {
            var filter = ExtractElementByType<ParameterFilterElement>(doc, f => f.Name.Equals(filterName, StringComparison.OrdinalIgnoreCase));
            if (filter == null) 
            {
                filter = ParameterFilterElement.Create(doc, filterName, categories);
            } else
            {
                filter.SetCategories(categories);
            }
            return filter;

        }

        public static void AddFilterToView(ParameterFilterElement filter, View view)
        {
            if(!view.GetFilters().Contains(filter.Id)) view.AddFilter(filter.Id);
        }

        public static T ExtractElementByType<T>(Document doc, Func<T,bool> predicate) where T : Element
        {
            T element = new FilteredElementCollector(doc)
            .OfClass(typeof(T))
            .Cast<T>()
            .FirstOrDefault(predicate);
            return element;
        }

        public static Dictionary<FamilyMode,IList<Element>> GetGroupedElementsByFamilyGroup(Document doc, List<Element> inElements)
        {
            if (inElements == null || inElements.Count == 0) return [];
            var filterFactory = new RevitFilterFactory();
            var elements = inElements;
            HashSet<BuiltInCategory> categories = new();
            ICableSystemCategory filter = null;
            RevitFilterManager filterManager = new(doc);
            Dictionary<FamilyMode, IList<Element>> groupedElements = new();
            
                    categories = RevitCategoriesService.ExtractCategoriesFromElements(elements);
                    filter = filterFactory.GetFilter(categories);
                    groupedElements = filterManager.GetCableElementsSelected(filter, elements);
            return groupedElements;
        }
    }
}
