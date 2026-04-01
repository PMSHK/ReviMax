using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Models.Annotations;

namespace ReviMax.DTO.Annotations
{
    internal class ReviLineDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Step { get; set; } = 0d;
        public double Offset { get; set; } = 0d;
        public double GlyphSize { get; set; } = 100d;
        public FamilyGroupDto Family { get; set; } = new ();
        public IList<ReviLineSegmentDto> Segments { get; set; } = [
                new ReviLineSegmentDto()
                {
                    Type = ReviLinePatternSegmentType.Dash,
                    LengthMm = 5.0
                }
                ];
        public byte Weight { get; set; } = (byte)1;
        public SymbolColorDto Color { get; set; } = new SymbolColorDto { R=0, G=0, B=0 };

        public override string ToString()
        {
            var segmentsStr = string.Join(", ", Segments.Select(s => s.ToString()));
            return $"ReviLineDto: Name: {Name}, CategoryId: {CategoryId}, Step: {Step}, Offset: {Offset}, GlyphSize: {GlyphSize}, Weight: {Weight}, Color: {Color}, Family: {Family}, Segments: [{segmentsStr}]";
        }
    }
}
