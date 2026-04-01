using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using ReviMax.Config;
using ReviMax.Core.Elements.Filters;
using ReviMax.Extensions;
using ReviMax.Models.Annotations;

namespace ReviMax.Services
{
    internal class RevitFilterManager
    {
        public Document Doc {  get; set; }
        public RevitFilterManager(Document doc) { Doc = doc; }

        public List<Category> GetLineStyles()
        {
            Category category = Doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
            List<Category> list = category.SubCategories.Cast<Category>().ToList();
            string info = string.Join(", ", list.Select(c => c.Name));
            return category.SubCategories.Cast<Category>().ToList();
        }

        public IList<Element> GetCableSystems()
        {
            View view = Doc.GetActiveView();
            var filter = new ElementMulticategoryFilter(new []
            {
                BuiltInCategory.OST_CableTray,
                BuiltInCategory.OST_Conduit,
                BuiltInCategory.OST_CableTrayFitting,
                BuiltInCategory.OST_ConduitFitting
                
            });

            FilteredElementCollector collector = new (Doc, view.Id);
            IList<Element> elements = collector
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();
            string info = string.Join(", ", elements.Select(e => e.Name));
            ReviMaxLog.Information($"RevitFilterManager. Found {elements.Count}. Found cable systems: {info}");
            TaskDialog.Show("Revit Plugin ", $"Found cable systems: {string.Join(", ",elements.Select(e => e.Name))}");
            return elements;
        }

        public IList<Element> GetCableElementsByCategory(BuiltInCategory bic)
        {
            View view = Doc.GetActiveView();
            var filter = new ElementCategoryFilter(bic);
            FilteredElementCollector collector = new (Doc, view.Id);
            IList<Element> elements = collector
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();
            
            TaskDialog.Show("Revit Plugin ", $"Found elements in category {bic}: {string.Join(", ",elements.Select(e => e.Name))}");
            return elements;
        }

        public Dictionary<FamilyMode, IList<Element>> GetCableElementsAll(ICableSystemCategory category) 
        {
            Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> filter = category.GetGroupedCategories();
            var result = new Dictionary<FamilyMode, IList<Element>>();
            foreach (var group in filter)
            {
                FamilyMode groupName = group.Key;
                IEnumerable<BuiltInCategory> categories = group.Value;
                var _filter = new ElementMulticategoryFilter(categories.ToArray());
                var elements = FilterElements(_filter);
                result[groupName] = elements;
                ReviMaxLog.Information($"Group: {groupName}, Categories: {string.Join(", ", categories)}, Found elements: {string.Join(", ", elements.Select(e => e.Name))}");
            }
            return result;
        }

        public Dictionary<FamilyMode, IList<Element>> GetCableElementsSelected(ICableSystemCategory category, List<Element> elements)
        {
            Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> filter = category.GetGroupedCategories();
            var result = new Dictionary<FamilyMode, IList<Element>>();
            foreach (var group in filter)
            {
                FamilyMode groupName = group.Key;
                HashSet<BuiltInCategory> categories = group.Value.ToHashSet();
                var groupElements = elements
                    .Where(e => e?.Category != null)
                    .Where(e => categories.Contains((BuiltInCategory)e.Category.Id.IntegerValue))
                    .ToList();
                result[groupName] = groupElements;
                ReviMaxLog.Information($"Group: {groupName}, Categories: {string.Join(", ", categories)}, Found elements: {string.Join(", ", elements.Select(e => e.Name))}");
            }
            return result;
        }

        public IList<Element> GetCableElementsByCategory(ICableSystemCategory categoty)
        {
            BuiltInCategory[] bic = categoty.GetCategory().ToArray();

            var filter = new ElementMulticategoryFilter(bic);
            var elements = FilterElements(filter);

            return elements;
        }

        private IList<Element> FilterElements(ElementMulticategoryFilter filter)
        {
            View view = Doc.GetActiveView();
            FilteredElementCollector collector = new(Doc, view.Id);
            IList<Element> elements = collector
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();
            return elements;
        }

        
    }
}
