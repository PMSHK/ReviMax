using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Enums;

namespace ReviMax.Core.Filters
{
    internal class CableTraysConduitsFilter : ICableSystemCategory
    {
        public IEnumerable<BuiltInCategory> GetCategory()
        {
            return 
            [
                BuiltInCategory.OST_CableTray,
                BuiltInCategory.OST_Conduit,
            ];
        }
        public Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> GetGroupedCategories()
        {
            return new Dictionary<FamilyMode, IEnumerable<BuiltInCategory>>
            {
                [FamilyMode.STRAIGHT_CABLE_SYSTEMS] = [BuiltInCategory.OST_CableTray, BuiltInCategory.OST_Conduit]
            };
        }

        public bool CanHandle(HashSet<BuiltInCategory> categories)
        {
            bool hasTray = categories.Contains(BuiltInCategory.OST_CableTray);

            bool hasConduit = categories.Contains(BuiltInCategory.OST_Conduit);
            return (hasTray || hasConduit) || (hasTray && hasConduit);
        }

        public override string ToString()
        {
            return nameof(CableTraysConduitsFilter);
        }
    }
}
