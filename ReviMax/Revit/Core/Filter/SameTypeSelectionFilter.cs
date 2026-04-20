using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace ReviMax.Revit.Core.Filter
{
    internal class SameTypeSelectionFilter : ISelectionFilter
    {
        private readonly ElementId _typeId;

        public SameTypeSelectionFilter(ElementId typeId)
        {
            _typeId = typeId;
        }

        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            return elem.GetTypeId() == _typeId;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
