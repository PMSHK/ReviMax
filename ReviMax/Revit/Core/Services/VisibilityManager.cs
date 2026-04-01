using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using ReviMax.Core.Config;

namespace ReviMax.Revit.Core.Services
{
    internal class VisibilityManager
    {
        public VisibilityManager() { }

        public void HideElementInView(ElementId elementId, View view)
        {
            if (view == null || elementId == null) return;
            try
            {
                view.HideElements(new List<ElementId> { elementId });
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Error hiding element {elementId.IntegerValue} in view {view.Id.IntegerValue}: {ex.Message}");
            }
        }

        public void UnhideElementInView(ElementId elementId, View view)
        {
            if (view == null || elementId == null) return;
            try
            {
                view.UnhideElements(new List<ElementId> { elementId });
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Error unhiding element {elementId.IntegerValue} in view {view.Id.IntegerValue}: {ex.Message}");
            }
        }
    }
}
