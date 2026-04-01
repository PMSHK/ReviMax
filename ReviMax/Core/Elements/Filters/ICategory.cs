using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Models.Annotations;

namespace ReviMax.Core.Elements.Filters
{
    internal interface ICategory<T>
    {
        IEnumerable<T> GetCategory();
        Dictionary<FamilyMode, IEnumerable<T>> GetGroupedCategories();

        bool CanHandle(HashSet<T> elements);
    }
}
