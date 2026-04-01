using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Utils.Converter;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.Models;
using ReviMax.GostSymbolManager.DTO.Annotations;

namespace ReviMax.GostSymbolManager.Mapper
{
    internal static class ReviLineMapper
    {
        public static ReviLine ToModel(this ReviLineDto dto)
        {
            if (dto == null) return new ReviLine();
            var dtoSegments = dto.Segments.Select(s => s.ToModel()).ToList();
            var segments = NormalizeSegments(dtoSegments);
            return new ReviLine
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Family = dto.Family.ToModel(),
                Weight = dto.Weight,
                Color = dto.Color.ToModel(),
                Segments = segments,
                Step = dto.Step.MillimetersToFeet(),
                Offset = dto.Offset.MillimetersToFeet(),
                GlyphSize = dto.GlyphSize.MillimetersToFeet(),

            };
        }
        public static ReviLineDto ToDto(this ReviLine model)
        {
            if (model == null) return new ReviLineDto();
            return new ReviLineDto
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Family = model.Family.ToDto(),
                Weight = model.Weight,
                Color = model.Color.ToDto(),
                Segments = model.Segments.Select(s => s.ToDto()).ToList(),
                Step = model.Step.FeetToMillimeters(),
                Offset = model.Offset.FeetToMillimeters(),
                GlyphSize = model.GlyphSize.FeetToMillimeters(),

            };
        }

        private static ReviLineSegment ToModel(this ReviLineSegmentDto dto)
        {
            if (dto == null) return new ReviLineSegment();
            return new ReviLineSegment
            {
                Type = dto.Type,
                Length = dto.LengthMm.MillimetersToFeet()
            };
        }
        private static ReviLineSegmentDto ToDto(this ReviLineSegment model)
        {
            if (model == null) return new ReviLineSegmentDto();
            return new ReviLineSegmentDto
            {
                Type = model.Type,
                LengthMm = model.Length.FeetToMillimeters()
            };
        }

        private static SymbolColor ToModel(this SymbolColorDto dto)
        {
            if (dto == null) return new SymbolColor(0, 0, 0);
            return new SymbolColor(dto.R, dto.G, dto.B);
        }

        private static SymbolColorDto ToDto(this SymbolColor model)
        {
            if (model == null) return new SymbolColorDto();
            return new SymbolColorDto
            {
                R = model.R,
                G = model.G,
                B = model.B
            };
        }



        private static IList<ReviLineSegment> NormalizeSegments(
            IEnumerable<ReviLineSegment>? input,
            double defaultDashLen = 5.0,
            double defaultSpaceLen = 2.0,
            double minLen = 1.0)
        {
            if (input == null)
                return DefaultSegments(defaultDashLen.MillimetersToFeet(), defaultSpaceLen.MillimetersToFeet());

            var src = input
                .Where(s => s != null)
                .Select(s =>
                {
                    var len = s.Length <= 0 ? minLen.MillimetersToFeet() : s.Length;
                    var type = s.Type == ReviLinePatternSegmentType.Invalid
                        ? ReviLinePatternSegmentType.Dash
                        : s.Type;

                    return new ReviLineSegment { Type = type, Length = len };
                });

            var result = new List<ReviLineSegment>();

            bool needNonSpace = true; // всегда начинаем с non-space

            foreach (var seg in src)
            {
                bool isSpace = seg.Type == ReviLinePatternSegmentType.Space;

                if (needNonSpace)
                {
                    if (isSpace)
                    {
                        // space там, где нужен non-space → просто игнорируем
                        continue;
                    }

                    // подошёл non-space
                    result.Add(seg);
                    needNonSpace = false; // дальше нужен space
                }
                else
                {
                    // нужен space
                    if (!isSpace)
                    {
                        // вместо "два non-space подряд" вставляем пробел и потом текущий non-space
                        result.Add(new ReviLineSegment
                        {
                            Type = ReviLinePatternSegmentType.Space,
                            Length = defaultSpaceLen
                        });

                        result.Add(seg);

                        // мы добавили non-space, значит теперь опять нужен space
                        needNonSpace = false;
                    }
                    else
                    {
                        // подошёл space
                        result.Add(seg);
                        needNonSpace = true; // дальше нужен non-space
                    }
                }
            }

            // Если в итоге нет ни одного non-space — возвращаем дефолт
            if (result.Count == 0)
                return DefaultSegments(defaultDashLen.MillimetersToFeet(), defaultSpaceLen.MillimetersToFeet());

            // Гарантируем окончание на space:
            if (result[^1].Type != ReviLinePatternSegmentType.Space)
            {
                result.Add(new ReviLineSegment
                {
                    Type = ReviLinePatternSegmentType.Space,
                    Length = defaultSpaceLen.MillimetersToFeet()
                });
            }

            return result;
        }


        private static IList<ReviLineSegment> DefaultSegments(double dashLen = 5.0, double spaceLen = 2.0) => new List<ReviLineSegment>
            {
                new ReviLineSegment { Type = ReviLinePatternSegmentType.Dash,  Length = dashLen.MillimetersToFeet() },
                new ReviLineSegment { Type = ReviLinePatternSegmentType.Space, Length = spaceLen.MillimetersToFeet() },
            };

        private static FamilyGroup ToModel(this FamilyGroupDto dto)
        {
            if (dto == null) return new FamilyGroup();
            return new FamilyGroup
            {
                Family = dto.Family.ToModel(),
                FamilyMode = dto.FamilyMode
            };
        }

        private static FamilyGroupDto ToDto(this FamilyGroup model)
        {
            if (model == null) return new FamilyGroupDto();
            return new FamilyGroupDto
            {
                Family = model.Family.ToDto(),
                FamilyMode = model.FamilyMode
            };
        }

        private static FamilyReferenceDto ToDto(this FamilyReference model) 
        {
            if (model == null) return new FamilyReferenceDto();
            return new FamilyReferenceDto
            {
                FamilyId = model.FamilyId,
                FamilyName = model.FamilyName,
                FamilyType = model.FamilyType
            };
        }

        private static FamilyReference ToModel(this FamilyReferenceDto dto) 
        {
            if (dto == null) return new FamilyReference();
            return new FamilyReference
            {
                FamilyId = dto.FamilyId,
                FamilyName = dto.FamilyName,
                FamilyType = dto.FamilyType
            };
        }

    }
}
