using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Autodesk.Revit.UI;

namespace ReviMax.Revit.Core.Bridge
{
    public class RevitDispatcherService
    {
        private readonly RevitRequesHandler _handler;
        private readonly ExternalEvent _externalEvent;

        public RevitDispatcherService()
        {
            Dispatcher uiDispatcher = Dispatcher.CurrentDispatcher;

            _handler = new RevitRequesHandler(uiDispatcher);
            _externalEvent = ExternalEvent.Create(_handler);
        }

        public void Request(Action<UIApplication> request, Action? callback = null)
        {
            _handler.Enqueue(request, callback);
            _externalEvent.Raise();
        }
    }
}
