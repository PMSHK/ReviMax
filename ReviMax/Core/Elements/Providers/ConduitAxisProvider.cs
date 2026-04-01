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
