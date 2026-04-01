using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Elements.Filters;

namespace ReviMax.Models.Annotations
{
    public class ReviLine: ICloneable
    {
        private byte _weight = 1;
        private double _step = 0;
        private double _offset = 0;
        private double _glyphSize = 0;
        public string Name { get; set; } = string.Empty;
        public FamilyGroup Family { get; set; } = new();
        public int CategoryId { get; set; }
        public IList<ReviLineSegment> Segments { get; set; } = [
                new ReviLineSegment()
                {
                    Type = ReviLinePatternSegmentType.Dash,
                    Length = 5.0
                }
                ];
        public byte Weight { get => _weight; set => NormolizeWeight(value); }
        public SymbolColor Color { get; set; } = new SymbolColor(0, 0, 0);
        public double Step{ get => _step; set => _step = value; }
        public double Offset{ get => _offset; set => _offset = value; }
        public double GlyphSize { get => _glyphSize; set { if (_glyphSize != value) _glyphSize = value ; } }
        public ReviLine() { }

        public ReviLine(int id) 
        {
            CategoryId = id;
        }
        public ReviLine(int id, string? name = null, byte? weight = null, FamilyGroup? familyGroup = null, IList<ReviLineSegment>? segments = null, SymbolColor? color = null)
        {

            CategoryId = id;
            Name = name ?? string.Empty;
            Family = familyGroup ?? new();
            Segments = segments ?? [
                new ReviLineSegment()
                {
                    Type = ReviLinePatternSegmentType.Dash,
                    Length = 5.0
                }
                ];
            _weight = NormolizeWeight(weight??1);
            Color = color ?? new SymbolColor(0, 0, 0);

        }

        public ReviLine(ReviLine source) {             
            Name = source.Name;
            Family = source.Family;
            CategoryId = source.CategoryId;
            Segments = source.Segments.Select(s => new ReviLineSegment() { Type = s.Type, Length = s.Length }).ToList();
            Weight = source.Weight;
            Color = source.Color;
            Step = source.Step;
            Offset = source.Offset;
            GlyphSize = source.GlyphSize;
        }

        public bool IsValidSegments()
        {
            if (Segments.Count == 0 || Segments.Count % 2 != 0 || !Segments.Last().Type.Equals(ReviLinePatternSegmentType.Space))
                return false;
            if (Segments.Count == 1 && Segments[0].Type == ReviLinePatternSegmentType.Space)
                return false;
            return true;
        }

        private byte NormolizeWeight(byte weight)
        {
            return weight switch
            {
                <= 0 => 1,
                > 16 => 16,
                _ => weight
            };
        }

        public override string ToString()
        {
            return $"ReviLine: Name={Name}, FamilyGroup= {Family}, Weight={Weight}, Color=({Color.R},{Color.G},{Color.B}), Segments=[{string.Join(", ", Segments.Select(s => $"(Type={s.Type}, Length={s.Length})"))}]," +
                $"Glyph size: {GlyphSize}, Step: {Step}, Offset: {Offset} ";
        }

        public bool Filled()
        {
            return !string.IsNullOrEmpty(Name) && Family.Filled();
        }

        public void CoppyFrom(ReviLine source)
        {
            if (ReferenceEquals(this, source)) return;
            Name = source.Name;
            if (source.Family.Filled()) { Family = source.Family; }  
            CategoryId = source.CategoryId;
            if(source.Segments == null || source.Segments.Count==0)
            {
                Segments = new List<ReviLineSegment>();
            } else { Segments = source.Segments.Select(s => new ReviLineSegment() { Type = s.Type, Length = s.Length }).ToList(); }
            
            Weight = source.Weight;
            Color = source.Color;
            Step = source.Step;
            Offset = source.Offset;
            GlyphSize = source.GlyphSize;
        }

        public Object Clone()
        {
            return new ReviLine(this);
        }
    }
}
