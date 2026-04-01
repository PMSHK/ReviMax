using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.Revit.Core.Services
{
    internal class RevitCategoriesService
    {
        public static HashSet<BuiltInCategory> ExtractCategoriesFromElements(List<Element> elements)
        {
            var categories = new HashSet<BuiltInCategory>();
            foreach (var element in elements)
            {
                if (element != null && element.Category != null)
                {
                    categories.Add(element.Category.BuiltInCategory);
                }
            }
            return categories;
        }
    }
}
