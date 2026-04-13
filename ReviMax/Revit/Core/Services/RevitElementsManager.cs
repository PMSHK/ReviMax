using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;

namespace ReviMax.Revit.Core.Services
{
    public class RevitElementsManager
    {

        public static void ShowElementsOnView(List<ElementId> elements, View activeView)
        {
            if (elements == null || elements.Count == 0) return;
            if (activeView == null) return;

            activeView.UnhideElements(elements);

            ReviMaxLog.Information($"Showing {elements.Count} elements on view {activeView.Name}");
        }

        public static void HideElementsOnView(List<ElementId> elements, View activeView)
        {
            if (elements == null || elements.Count == 0) return;
            if (activeView == null) return;
            
            activeView.HideElements(elements);

            ReviMaxLog.Information($"Hiding {elements.Count} elements on view {activeView.Name}");
        }

    }
}
