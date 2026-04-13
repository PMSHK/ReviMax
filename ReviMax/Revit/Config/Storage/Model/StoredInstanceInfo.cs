using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.Revit.Config.Storage.Model
{
    public struct StoredInstanceInfo
    {
        public ElementId InstanceId { get; set; }
        public string RunId { get; set; }
        public int ViewId { get; set; }
        public List<ElementId> SourceIds { get; set; } = new List<ElementId>();

        public StoredInstanceInfo(ElementId instanceId, string runId, int viewId, List<ElementId> sourceIds)
        {
            InstanceId = instanceId;
            RunId = runId;
            ViewId = viewId;
            SourceIds = sourceIds;
        }

    }
}
