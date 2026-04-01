using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Models.Annotations;

namespace ReviMax.Core.Elements.Filters
{
    internal class CableTrayFilter : ICableSystemCategory
    {
        public IEnumerable<BuiltInCategory> GetCategory()
        {
            return 
            [
                BuiltInCategory.OST_CableTray,
                BuiltInCategory.OST_CableTrayFitting
            ];
        }
        public Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> GetGroupedCategories()
        {
            return new Dictionary<FamilyMode, IEnumerable<BuiltInCategory>>
            {
                [FamilyMode.TRAY] = [BuiltInCategory.OST_CableTray, BuiltInCategory.OST_CableTrayFitting]
            };
        }

        public bool CanHandle(HashSet<BuiltInCategory> categories)
        {
            bool hasTray = categories.Contains(BuiltInCategory.OST_CableTray) ||
                   categories.Contains(BuiltInCategory.OST_CableTrayFitting);

            bool hasConduit = categories.Contains(BuiltInCategory.OST_Conduit) ||
                              categories.Contains(BuiltInCategory.OST_ConduitFitting);
            return hasTray && !hasConduit;
        }

        public override string ToString()
        {
            return nameof(CableTrayFilter);
        }
    }
}
