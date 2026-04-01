using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Models.Annotations;

namespace ReviMax.DTO.Annotations
{
    internal class ReviLineSegmentDto
    {
        public ReviLinePatternSegmentType Type { get; set; }
        public double LengthMm { get; set; }
        public override string ToString()
        {
            return $"ReviLineSegmentDto: Type: {Type}, LengthMm: {LengthMm}";
        }
    }
}
