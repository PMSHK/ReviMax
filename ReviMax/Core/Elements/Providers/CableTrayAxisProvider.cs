using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using ReviMax.Config;
using ReviMax.core.Elements;
using ReviMax.Models.Revit;
using ReviMax.Services;

namespace ReviMax.Core.Elements.Providers
{
    internal class CableTrayAxisProvider : AxisProvider
    {
        public CableTrayAxisProvider(Document doc) : base(doc)
        {}
        public override IEnumerable<AxisSegment> GextAxes(Element element, View view)
        {
            if (element is not CableTray tray) yield break;
            Curve axis = CableSystemManager.GetAxisProjection(tray);
            if (!IsValidCurve(axis)) 
            {
                {
                    ReviMaxLog.Warning($"ReviDrawingManager. Invalid axis curve for element ID {element.Id}.");
                    yield break;
                }
            }
            yield return new AxisSegment(tray, axis);
        }

        public override bool CanHandle(Element element)
        {
            return element is CableTray;
        }
    }
}
