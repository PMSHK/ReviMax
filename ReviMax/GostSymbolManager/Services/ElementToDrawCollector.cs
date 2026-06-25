using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using ReviMax.Core.Enums;
using ReviMax.Core.Filters;
using ReviMax.GostSymbolManager.Models;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Model;

namespace ReviMax.GostSymbolManager.Services
{
    internal class ElementToDrawCollector
    {
        private CableSystemService _cableSystemService;

        public ElementToDrawCollector(CableSystemService cableSystemService)
        {
            _cableSystemService = cableSystemService;
        }

        public ElementToDraw GetElementsToDraw(Document doc, ICableSystemCategory filter)
        {
            var elements = _cableSystemService.GetCableSystemsByCategory(filter);

            var elementToDraw = new ElementToDraw()
            {
                LinkedDocumentInfo = new DocumentInfo(doc),
                Elements = elements,
                Type = RMDocumentType.CURRENT,
            };
            return elementToDraw;
        }

        public List<ElementToDraw> GetLinkedElementsToDraw(Document doc, ICableSystemCategory filter)
        {
            var result = new List<ElementToDraw>();
            //var linkedDocumentsWithInstances = LinkedDocManager.GetLinkedDocumentsInfo(doc);
            foreach (var linkedDocInfo in doc.GetLinkedDocumentsInfo())
            {
                var linkedElements = _cableSystemService.GetCableSystemsByCategory(filter, linkedDocInfo.Document);
                if (linkedElements == null) continue;
                foreach (var linkedElement in linkedElements)
                {
                    result.Add(new ElementToDraw()
                    {
                        LinkedDocumentInfo = linkedDocInfo,
                        Elements = new Dictionary<FamilyMode, IList<Element>> { { linkedElement.Key, linkedElement.Value } },
                        Type = RMDocumentType.LINKED,
                    });
                }
            }
            return result;
        }
    }
}
