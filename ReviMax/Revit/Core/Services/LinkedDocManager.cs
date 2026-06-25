using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Enums;
using ReviMax.Core.Filters;
using ReviMax.GostSymbolManager.Models;
using ReviMax.Revit.Model;

namespace ReviMax.Revit.Core.Services
{
    public static class LinkedDocManager
    {
        public static IEnumerable<RevitLinkInstance> FindLinkedInstances(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<RevitLinkInstance> linkedDocs = collector
                .OfClass(typeof(RevitLinkInstance))
                .Cast<RevitLinkInstance>()
                .ToList();
            foreach (RevitLinkInstance link in linkedDocs)
            {
                if (link == null) continue;
                yield return link;
            }
        }

        public static IEnumerable<T> GetLinkedDocuments<T>(this IEnumerable<RevitLinkInstance> linkInstances) where T : Document
        {
            foreach (var link in linkInstances)
            {
                T? linkDoc = link.GetLinkDocument() as T;
                if (linkDoc == null)
                    continue;
                yield return linkDoc;
            }
        }

        public static IEnumerable<DocumentInfo> GetLinkedDocumentsInfo(this Document doc)
        {
            var linkedInstances = FindLinkedInstances(doc);
            foreach (var instance in linkedInstances)
            {
                var linkedDoc = instance.GetLinkDocument();
                if (linkedDoc == null)
                    continue;
            yield return new DocumentInfo(instance, linkedDoc);
            }
        }
    }
}
