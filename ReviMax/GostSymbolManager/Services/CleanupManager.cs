using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Revit.Config.Storage;
using ReviMax.Revit.Config.Storage.Model;
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

            var elements = ReviMaxStorage.GetInstanceByRunidActiveView(runId,Doc.ActiveView);

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
                RevitElementsManager.ShowElementsOnView(elements, doc.ActiveView);
            }));
            
            return idList.Count;
        }

        public int DeleteReviMaxElementsByRunId(string runId, Dictionary<string,List<StoredInstanceInfo>> elements)
        {
            if (string.IsNullOrEmpty(runId) || elements == null) return 0;
            List<StoredInstanceInfo> cableSystemElements = new();
            List<ElementId> elementsToDelete = new();
            foreach (var element in elements.Where(el=> el.Key == runId))
            {
                cableSystemElements.AddRange(element.Value);
            }

            if(cableSystemElements.Count == 0) return 0;
            TransactionManager.StartTransaction(Doc, "Delete ReviMax Transaction", (doc =>
            {
                foreach (var element in cableSystemElements)
                {
                    if (element.DocumentType == Models.RMDocumentType.LINKED)
                    {
                        doc.Delete(element.InstanceId);
                    }
                    if (element.DocumentType == Models.RMDocumentType.CURRENT)
                    {
                        doc.Delete(element.InstanceId);
                        elementsToDelete.AddRange(element.SourceIds);
                    }
                }
                RevitElementsManager.ShowElementsOnView(elementsToDelete, doc.ActiveView);
            }));
            return elementsToDelete.Count;
        }
    }
}
