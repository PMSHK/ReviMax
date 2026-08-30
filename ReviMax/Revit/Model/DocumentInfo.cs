using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Models;

namespace ReviMax.Revit.Model
{
    public class DocumentInfo
    {
        public RevitLinkInstance? LinkInstance { get; }
        public Document Document { get; }
        public RMDocumentType Type { get; set; } = RMDocumentType.CURRENT;

        public DocumentInfo(RevitLinkInstance linkInstance, Document document)
        {
            LinkInstance = linkInstance;
            Document = document;
        }
        public DocumentInfo(Document document)
        {
            Document = document;
        }
        public DocumentInfo(Document document, RMDocumentType type)
        {
            Document = document;
            Type = type;
        }
    }
}
