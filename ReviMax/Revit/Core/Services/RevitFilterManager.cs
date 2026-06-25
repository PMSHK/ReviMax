using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using ReviMax.CircuitAnalyzeManager.Models;
using ReviMax.CircuitAnalyzeManager.Models.Graph;
using ReviMax.Core.Config;
using ReviMax.Core.Enums;
using ReviMax.Core.Extensions;
using ReviMax.Core.Filters;
using ReviMax.Core.Utils.Converter;
using ReviMax.GostSymbolManager.Models;
using ReviMax.GostSymbolManager.Services;
using ReviMax.Handlers;
using ReviMax.Revit.Model;

namespace ReviMax.Revit.Core.Services
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
            FilteredElementCollector collector = new (Doc);
            IList<Element> elements = collector
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();
            
            TaskDialog.Show("Revit Plugin ", $"Found elements in category {bic}: {string.Join(", ",elements.Select(e => e.Name))}");
            return elements;
        }
        public IList<Element> GetCableElementsByCategoryByActiveView(BuiltInCategory bic)
        {
            View view = Doc.GetActiveView();
            var filter = new ElementCategoryFilter(bic);
            FilteredElementCollector collector = new(Doc, view.Id);
            IList<Element> elements = collector
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();

            TaskDialog.Show("Revit Plugin ", $"Found elements in category {bic}: {string.Join(", ", elements.Select(e => e.Name))}");
            return elements;
        }

        public IList<Element> GetCableElementsByCategory(BuiltInCategory bic, View view)
        {
            var filter = new ElementCategoryFilter(bic);
            FilteredElementCollector collector = new(Doc, view.Id);
            IList<Element> elements = collector
                .WherePasses(filter)
                .WhereElementIsNotElementType()
                .ToElements();

            TaskDialog.Show("Revit Plugin ", $"Found elements in category {bic}: {string.Join(", ", elements.Select(e => e.Name))}");
            return elements;
        }

        public Dictionary<FamilyMode, IList<Element>> GetCableElementsAll(ICableSystemCategory category, Document? doc = null) 
        {
            Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> filter = category.GetGroupedCategories();
            var result = new Dictionary<FamilyMode, IList<Element>>();
            foreach (var group in filter)
            {
                FamilyMode groupName = group.Key;
                IEnumerable<BuiltInCategory> categories = group.Value;
                var _filter = new ElementMulticategoryFilter(categories.ToArray());

                    var elements = FilterElements(_filter, doc);
                    result[groupName] = elements;

            }
            return result;
        }

        public Dictionary<string, List<GraphCandidate>> GetElectricalElementsAll(ICableSystemCategory category, CircuitGraph? graph = null, double tolerance = 0.0)
        {
            var elementGroups = GetCableElementsAll(category);

            Dictionary<string, List<GraphCandidate>> result = new Dictionary<string, List<GraphCandidate>>();
            if (elementGroups != null)
            {
                var groupedElements = new Dictionary<string, List<GraphCandidate>>();

                foreach (var group in elementGroups)
                {
                    var resultList = new List<GraphCandidate>();

                    foreach (var e in group.Value)
                    {
                        try
                        {
                            var candidate = GraphAnalyzer.FindGraphCandidate(
                                            e,
                                            graph,
                                            tolerance);

                            if (e == null) continue;
                            if (!e.IsValidObject) continue;
                            if (!RevitElementsManager.HasPlaced(e)) continue;
                            if (RevitElementsManager.IsNestedSubComponent(e)) continue;
                            if (string.IsNullOrEmpty(e.Name)) continue;
                            if (candidate == null) continue;

                            resultList.Add(candidate);
                        }
                        catch (Exception ex)
                        {
                            ReviMaxLog.Error($"Element check failed. Id: {e?.Id.IntegerValue}, Name: {RevitElementsManager.SafeName(e)}, Error: {ex}");
                        }
                    }

                    if (resultList.Any())
                        groupedElements[group.Key.GetDescription()] = resultList;
                }

                if (groupedElements.Any())
                {
                    var logMessages = groupedElements.Select(g =>
                        $"{g.Key}: {string.Join(", ", g.Value.Select(el => string.Concat( el.ElementId, " ",el.LocationType.ToString())))}"
                    );

                    ReviMaxLog.Information("Found elements: " + string.Join(" | ", logMessages));
                }
                else
                {
                    ReviMaxLog.Information("No matching elements found within the specified tolerance.");
                }

                return groupedElements;
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

        private IList<Element> FilterElements(ElementMulticategoryFilter filter, Document? doc = null)
        {
            if (doc == null)
            {
                View view = Doc.GetActiveView();
                FilteredElementCollector collector = new(Doc, view.Id);
                IList<Element> elements = collector
                    .WherePasses(filter)
                    .WhereElementIsNotElementType()
                    .ToElements();
                return elements;
            }
            else
            {
                FilteredElementCollector collector = new(Doc);
                IList<Element> elements = collector
                    .WherePasses(filter)
                    .WhereElementIsNotElementType()
                    .ToElements();
                return elements;
            }
        }

    }
}
