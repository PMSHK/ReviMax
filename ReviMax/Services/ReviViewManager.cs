using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Config;

namespace ReviMax.Services
{
    internal static class ReviViewManager
    {
        public static View GetActiveView(this Document doc)
        {
            if (doc == null)
            {
                ReviMaxLog.Warning($"ReviViewManager. Document {nameof(doc)} cannot be null");
                throw new ArgumentNullException(nameof(doc), "Document cannot be null.");
            }
            try
            {
                return doc.ActiveView;
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"ReviViewManager. Failed to get active view from document {doc.Title}.", ex);
                throw new InvalidOperationException($"ReviViewManager. Failed to get active view from document {doc.Title}.", ex);
            }
        }
    }
}
