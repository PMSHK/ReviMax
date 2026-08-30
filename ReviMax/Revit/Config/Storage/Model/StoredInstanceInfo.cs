using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Models;

namespace ReviMax.Revit.Config.Storage.Model
{
    public struct StoredInstanceInfo
    {
        public ElementId InstanceId { get; set; }
        public string RunId { get; set; }
        public int ViewId { get; set; }
        public List<ElementId> SourceIds { get; set; } = new List<ElementId>();
        public List<string> SourceUniqueIds { get; set; } = new List<string>();
        public RMDocumentType DocumentType { get; set; } = RMDocumentType.CURRENT;
        public string SymbolName { get; set; } = string.Empty;
        public Document Doc { get; set; }

        public StoredInstanceInfo(Document doc,ElementId instanceId, string runId, int viewId, List<ElementId> sourceIds, RMDocumentType documentType = RMDocumentType.CURRENT, List<string>? sourceUniqueIds = null, string symbolName = "")
        {
            Doc = doc;
            InstanceId = instanceId;
            RunId = runId;
            ViewId = viewId;
            SourceIds = sourceIds;
            DocumentType = documentType;
            SourceUniqueIds = sourceUniqueIds ?? new List<string>();
            SymbolName = symbolName;
        }

    }
}
