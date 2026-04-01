using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using ReviMax.Config;
using ReviMax.core.Elements;
using ReviMax.Models.Revit;
using ReviMax.Services;

namespace ReviMax.Core.Elements.Providers
{
    internal class CableTrayFittingAxisProvider : AxisProvider
    {
        public CableTrayFittingAxisProvider(Document doc) : base(doc)
        { }
        public override IEnumerable<AxisSegment> GextAxes(Element element, View view)
        {
            if (element is not FamilyInstance trayFitting) yield break;
            var axis = CableSystemManager.GetFittingPojectionLine(trayFitting);
            foreach (Curve curve in axis)
            {
                if (IsValidCurve(curve))
                {
                    yield return new AxisSegment(trayFitting, curve);
                }
                else
                {
                    ReviMaxLog.Warning($"ReviDrawingManager. Invalid axis curve for element ID {element.Id}.");
                }
            }
        }

        public override bool CanHandle(Element element)
        {
            return element is FamilyInstance fi &&
               fi.MEPModel?.ConnectorManager != null; ;
        }
    }
}
