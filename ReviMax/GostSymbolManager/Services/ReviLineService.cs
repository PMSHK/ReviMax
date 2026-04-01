using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Utils.Managers;
using System.IO;
using ReviMax.Core.Utils.Config;
using Autodesk.Revit.DB;
using ReviMax.Revit.Core.Services;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Mapper;

namespace ReviMax.GostSymbolManager.Services
{
    internal class ReviLineService
    {
        private Document _doc;
        private List<Category> _lines;
        public List<Category> LineStyles { get => _lines;}
        public Document Doc { get => _doc; set => _doc = value; }
        public ReviLineService(Document doc)
        {
            _doc = doc;
            _lines= new RevitFilterManager(doc).GetLineStyles();
        }
        public ReviLineService(Document doc, List<Category>? lineStyles)
        {
            _doc = doc;
            _lines = lineStyles??=[];
        }
        public ReviLine? LoadReviLine(string path)
        {
            if (!File.Exists(path) || Path.GetExtension(path) != ".json")
            {
                ReviMaxLog.Warning($"ReviLine file not found at {path} or invalid file type.");
                return null;
            }
            JsonConverter.Deserialize<ReviLineDto>(File.ReadAllText(path), out ReviLineDto? dto);
            return dto?.ToModel() ?? new ReviLine();
        }

        public void SaveReviLine(ReviLine line, string path, string fileName)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            var lineToSave = line.ToDto();
            PathManager.CreateDirectoryIfNotExist(path);
            File.WriteAllText(PathManager.GetFilePathInDirectory(path, fileName), JsonConverter.Serialize<ReviLineDto>(lineToSave));
            ReviMaxLog.Information($"ReviLine saved to {path}");
        }

        public ReviLine CreateReviLine(int id, string name, byte weight = 1, FamilyGroup? family = null, params (ReviLinePatternSegmentType type, double length)[]? segments)
        {
            ReviLine line = new ReviLine(id, name, weight);
            if (segments != null && segments.Length > 0)
            {
                line.Segments = CreateSegment(segments);
            }
            if (family != null) line.Family = family;
            return line;
        }

        public IList<ReviLineSegment> CreateSegment(params (ReviLinePatternSegmentType type, double length)[] segments)
        {
            IList<ReviLineSegment> lineSegments = new List<ReviLineSegment>();
            foreach (var segment in segments)
            {
                if (segment.length <=0) continue;
                lineSegments.Add(new ReviLineSegment { Type = segment.type, Length = segment.length });
            }
            return lineSegments.Count>0? lineSegments : [];
        }

        public void ChangeLineColor(ReviLine line, SymbolColor color)
        {
            line?.Color = color;
        }

        public ReviLine BuildReviLine(int CategoryId)
        {
            Category category = Category.GetCategory(Doc,new ElementId(CategoryId));
            return new(
                CategoryId, category.Name, 
                (byte?)category.GetLineWeight(GraphicsStyleType.Projection), 
                color: new SymbolColor(category.LineColor.Red, category.LineColor.Green, category.LineColor.Blue),
                segments: GetSegments(category));
        }

        public ReviLine BuildReviLine(string lineName, FamilyMode familyMode)
        {
            Category category = _lines.FirstOrDefault(l=>l.Name.Equals(lineName));
            if (category == null)
                throw new InvalidOperationException($"Line category '{lineName}' not found.");
            ReviMaxLog.Information($"Line category with name {lineName} {(category != null ? "found" : "not found")}, ID: {category.Id}.");
            int categoryId = category.Id.IntegerValue;
            ReviMaxLog.Information($"categoryId ok: {categoryId}");
            
            string name = category.Name;
            ReviMaxLog.Information($"name ok: {name}");

            byte? weight = (byte?)category.GetLineWeight(GraphicsStyleType.Projection);
            ReviMaxLog.Information($"weight ok: {weight}");

            var lineColor = category.LineColor;
            ReviMaxLog.Information($"lineColor ok: R={lineColor.Red}, G={lineColor.Green}, B={lineColor.Blue}");

            var color = new SymbolColor(lineColor.Red, lineColor.Green, lineColor.Blue);
            ReviMaxLog.Information("SymbolColor created");

            var segments = GetSegments(category);
            ReviMaxLog.Information($"segments created. null = {segments == null}");

            FamilyGroup familyGroup = new() { FamilyMode = familyMode };

            var line = new ReviLine(categoryId, name, weight, color: color, segments: segments, familyGroup:familyGroup);
            ReviMaxLog.Information("ReviLine created successfully");

            return line;

        }

        private List<ReviLineSegment> GetSegments(Category category)
        {
            try
            {
                ReviMaxLog.Information($"GetSegments start for '{category?.Name}'");

                ElementId patternId = category.GetLinePatternId(GraphicsStyleType.Projection);
                ReviMaxLog.Information($"PatternId: {patternId?.IntegerValue}");
                List<ReviLineSegment> lineSegments = new();

                if (patternId == null || patternId == ElementId.InvalidElementId)
                {
                    ReviMaxLog.Warning($"Invalid patternId for '{category?.Name}'");
                    return lineSegments;
                }

                LinePattern pattern = LinePatternElement.GetLinePattern(Doc, patternId);
                ReviMaxLog.Information($"Pattern is null: {pattern == null}");

                if (pattern == null)
                {
                    ReviMaxLog.Warning($"Pattern not found for '{category?.Name}', patternId={patternId.IntegerValue}");
                    return lineSegments;
                }

                IList<LinePatternSegment> segments = pattern.GetSegments();
                ReviMaxLog.Information($"Segments count: {segments?.Count}");

                if (segments == null || segments.Count == 0)
                {
                    ReviMaxLog.Information($"No segments for '{category?.Name}'");
                    return lineSegments;
                }

                foreach (LinePatternSegment s in segments)
                {
                    lineSegments.Add(new ReviLineSegment
                    {
                        Type = s.Type switch
                        {
                            LinePatternSegmentType.Dash => ReviLinePatternSegmentType.Dash,
                            LinePatternSegmentType.Space => ReviLinePatternSegmentType.Space,
                            LinePatternSegmentType.Dot => ReviLinePatternSegmentType.Dot,
                            _ => ReviLinePatternSegmentType.Invalid
                        },
                        Length = s.Length
                    });
                }

                return lineSegments;
            }
            catch (Exception ex)
            {
                ReviMaxLog.Error($"Error in GetSegments for category '{category?.Name}': {ex.Message}", ex);
                return [];
            }
        }

    }
}
