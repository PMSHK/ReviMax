using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Enums;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.Revit.Model;

namespace ReviMax.GostSymbolManager.Models
{
    public class ElementToDraw
    {
        public DocumentInfo? LinkedDocumentInfo { get; set; } = null;
        public Dictionary<FamilyMode, IList<Element>>? Elements { get; set; } = new();
        public RMDocumentType Type { get; set; } = RMDocumentType.CURRENT;
        public Color Color { get; set; } = new Color(0, 0, 0);
        public Transform Transform => LinkedDocumentInfo?.LinkInstance?.GetTotalTransform() ?? Transform.Identity;
        public Transform InverseTransform => LinkedDocumentInfo?.LinkInstance?.GetTotalTransform().Inverse ?? Transform.Identity;
        public SymbolColor ConvertColor => new SymbolColor
        {
            R = Color.Red,
            G = Color.Green,
            B = Color.Blue
        };
    }
}
