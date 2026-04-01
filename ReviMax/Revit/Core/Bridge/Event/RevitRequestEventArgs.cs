using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace ReviMax.Revit.Core.Bridge.Event
{
    public class RevitRequestEventArgs
    {
        public RevitRequesHandler Handler { get; set; }
        public ExternalEvent ExecuteEvent { get; set; }
    }
}
