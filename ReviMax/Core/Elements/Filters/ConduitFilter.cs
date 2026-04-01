using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Models.Annotations;

namespace ReviMax.Core.Elements.Filters
{
    internal class ConduitFilter : ICableSystemCategory
    {
        public IEnumerable<BuiltInCategory> GetCategory()
        {
            return 
            [
                BuiltInCategory.OST_Conduit,
                BuiltInCategory.OST_ConduitFitting
            ];
        }
        public Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> GetGroupedCategories()
        {
            return new Dictionary<FamilyMode, IEnumerable<BuiltInCategory>>
            {
                [FamilyMode.CONDUITS] = [BuiltInCategory.OST_Conduit, BuiltInCategory.OST_ConduitFitting]
            };
        }

        public bool CanHandle(HashSet<BuiltInCategory> categories)
        {
            bool hasTray = categories.Contains(BuiltInCategory.OST_CableTray) ||
                   categories.Contains(BuiltInCategory.OST_CableTrayFitting);

            bool hasConduit = categories.Contains(BuiltInCategory.OST_Conduit) ||
                              categories.Contains(BuiltInCategory.OST_ConduitFitting);
            return !hasTray && hasConduit;
        }
    }
}
