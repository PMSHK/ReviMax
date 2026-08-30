using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Services;

namespace ReviMax.Revit.Model
{
    public class ElementQuery
    {
        public Document? Doc { get; set; }

        public View? View => Doc?.GetActiveView();

        public List<BuiltInCategory> Categories { get; set; } = new();

        public List<string> FamilyNames { get; set; } = new();

        public List<ElementFilter> CustomFilters { get; set; } = new();

        public bool ExcludeElementTypes { get; set; } = true;
        
    }
}
