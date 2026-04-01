using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.GostSymbolManager.Models.Revit
{
    internal class AxisSegment
    {
        public Element Element { get; set; }
        public Curve Axis { get; set; }
        public XYZ StartPoint { get; set; }
        public XYZ EndPoint { get; set; }

        public double Length => Axis.Length;

        public AxisSegment() { }
        public AxisSegment(Element element, Curve axis)
        {
            Element = element;
            Axis = axis;
            StartPoint = axis.GetEndPoint(0);
            EndPoint = axis.GetEndPoint(1);
        }

        public override bool Equals(object? obj)
        {
            return obj is AxisSegment segment &&
                   EqualityComparer<Element>.Default.Equals(Element, segment.Element) &&
                   EqualityComparer<Curve>.Default.Equals(Axis, segment.Axis) &&
                   EqualityComparer<XYZ>.Default.Equals(StartPoint, segment.StartPoint) &&
                   EqualityComparer<XYZ>.Default.Equals(EndPoint, segment.EndPoint) &&
                   Length == segment.Length;
        }

        public override int GetHashCode()
        {
            int hashCode = -343656797;
            hashCode = hashCode * -1521134295 + EqualityComparer<Element>.Default.GetHashCode(Element);
            hashCode = hashCode * -1521134295 + EqualityComparer<Curve>.Default.GetHashCode(Axis);
            hashCode = hashCode * -1521134295 + EqualityComparer<XYZ>.Default.GetHashCode(StartPoint);
            hashCode = hashCode * -1521134295 + EqualityComparer<XYZ>.Default.GetHashCode(EndPoint);
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            return hashCode;
        }
    }
}
