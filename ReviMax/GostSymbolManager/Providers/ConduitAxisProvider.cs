using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.Models.Revit;
using ReviMax.GostSymbolManager.Services;

namespace ReviMax.GostSymbolManager.Providers
{
    internal class ConduitAxisProvider : AxisProvider
    {
        public ConduitAxisProvider(Document doc) : base(doc)
        { }
        public override IEnumerable<AxisSegment> GextAxes(Element element, View view)
        {
            if (element is not Conduit conduit) yield break;
            Curve axis = CableSystemManager.GetAxisProjection(conduit);
            if (!IsValidCurve(axis))
            {
                {
                    ReviMaxLog.Warning($"ReviDrawingManager. Invalid axis curve for element ID {element.Id}.");
                    yield break;
                }
            }
            yield return new AxisSegment(conduit, axis);
        }

        public override bool CanHandle(Element element)
        {
            return element is Conduit;
        }
    }
}
