using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;

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

    }
}
