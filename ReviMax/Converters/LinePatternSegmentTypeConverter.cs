using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Models.Annotations;


namespace ReviMax.Converters
{
    internal static class LinePatternSegmentTypeConverter
    {
        public static LinePatternSegmentType ToRevitLinePatternSegmentType(this ReviLinePatternSegmentType segmentType)
        {
            return segmentType switch
            {
                ReviLinePatternSegmentType.Dash => LinePatternSegmentType.Dash,
                ReviLinePatternSegmentType.Dot => LinePatternSegmentType.Dot,
                ReviLinePatternSegmentType.Space => LinePatternSegmentType.Space,
                _ => LinePatternSegmentType.Invalid
            };
        }
        public static ReviLinePatternSegmentType ToReviLinePatternSegmentType(this LinePatternSegmentType segmentType)
        {
            return segmentType switch
            {
                LinePatternSegmentType.Dash => ReviLinePatternSegmentType.Dash,
                LinePatternSegmentType.Dot => ReviLinePatternSegmentType.Dot,
                LinePatternSegmentType.Space => ReviLinePatternSegmentType.Space,
                _ => ReviLinePatternSegmentType.Invalid
            };
        }
    }
}
