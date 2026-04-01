using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.Models.Revit;

namespace ReviMax.GostSymbolManager.Providers
{
    internal class ConduitFittingsAxisProvider : AxisProvider
    {
        public ConduitFittingsAxisProvider(Document doc) : base(doc)
        { }
        public override IEnumerable<AxisSegment> GextAxes(Element element, View view)
        {
            if (element is not FamilyInstance conduitFitting) yield break;
            IEnumerable<Curve> axis = CableSystemManager.GetFittingPojectionLine(conduitFitting);
            foreach (Curve curve in axis)
            {
                if (IsValidCurve(curve))
                {
                    yield return new AxisSegment(conduitFitting, curve);
                } else
                {
                    ReviMaxLog.Warning($"ReviDrawingManager. Invalid axis curve for element ID {element.Id}.");
                }
            }
        }

        public override bool CanHandle(Element element)
        {
            return element is FamilyInstance fi &&
               fi.MEPModel?.ConnectorManager != null;
        }
    }
}
