using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Revit.Config.Storage;
using ReviMax.Revit.Core.Services;

namespace ReviMax.GostSymbolManager.Services
{
    internal class CleanupManager
    {
        private Document _doc;
        public Document Doc { get => _doc; }
        public CleanupManager(Document doc) 
        {
         _doc = doc;
        }
        public int DeleteReviMaxElement(
            string runId, 
            ElementId? viewId = null,
            List<BuiltInCategory>? categories = null
            )
        {
            categories ??= 
                            [
                                BuiltInCategory.OST_Lines,           
                                BuiltInCategory.OST_TextNotes,
                                BuiltInCategory.OST_DetailComponents,
                                BuiltInCategory.OST_FilledRegion,
                                BuiltInCategory.OST_GenericAnnotation
                            ];

            viewId ??= Doc.GetActiveView().Id;

            var idList = ReviMaxStorage.GetElementIdsByRunId
                (
                    Doc, 
                    runId, 
                    viewId, 
                    categories
                );
            TransactionManager.StartTransaction(Doc, "Delete ReviMax Transaction", (doc => 
            {
                doc.Delete(idList);
            }));
            return idList.Count;
        }
    }
}
