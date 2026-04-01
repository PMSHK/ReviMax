using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.Filters;

namespace ReviMax.GostSymbolManager.Providers.Factory
{
    internal class RevitFilterFactory
    {
        private List<ICableSystemCategory> _filters;

        public RevitFilterFactory() 
        {
            _filters = FindFilterClasses();
            ReviMaxLog.Information($"Found {_filters.Count} Filters: {string.Join(", ", _filters.Select(f => nameof(f)))}");
        }

        public ICableSystemCategory GetFilter(HashSet<BuiltInCategory> categories)
        {
            return _filters.FirstOrDefault(f => f.CanHandle(categories));
        }

        public List<ICableSystemCategory> FindFilterClasses()
        {
            return ReflectionClassFinder.GetChildForInterface<ICableSystemCategory>();
        }
    }
}
