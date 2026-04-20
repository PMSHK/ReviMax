using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Autodesk.Revit.UI;
using ReviMax.Core.Config;

namespace ReviMax.Revit.Core.Bridge
{
    public class RevitRequesHandler : IExternalEventHandler
    {
        private readonly Queue<(Action<UIApplication> request, Action? callback)> _queue = new();
        private readonly Dispatcher _uiDispatcher;

        public RevitRequesHandler(Dispatcher uiDispatcher) {  _uiDispatcher = uiDispatcher ?? throw new ArgumentNullException(nameof(uiDispatcher)); ; }
        public void Enqueue(Action<UIApplication> request, Action? callback = null)
        {
            lock (_queue)
            {
                _queue.Enqueue((request, callback));
            }
        }

        public void Execute(UIApplication app)
        {
            while (true) { 
            (Action<UIApplication> request, Action? callback)? item = null;

            lock (_queue)
            {
                if (_queue.Count > 0)
                    item = _queue.Dequeue();
            }

            if (item == null)
                return;

            try
            {
                ReviMaxLog.Information("ExternalEvent request started");
                item.Value.request(app);
                ReviMaxLog.Information("ExternalEvent request finished");
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error("Error in ExternalEvent request", ex);
                //TaskDialog.Show("Error in ExternalEvent", ex.ToString());
            }

            try
            {
                if (item.Value.callback != null)
                {
                    ReviMaxLog.Information("ExternalEvent callback started");
                        //item.Value.callback.Invoke();
                        _uiDispatcher.BeginInvoke(new Action(() =>
                        {
                            item.Value.callback.Invoke();
                        }));
                        ReviMaxLog.Information("ExternalEvent callback finished");
                }
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error("Error in ExternalEvent callback", ex);
                //TaskDialog.Show("Error in ExternalEvent callback", ex.ToString());
            }
            }
        }

        public string GetName() => "Revit Event Handler";

    }
}
