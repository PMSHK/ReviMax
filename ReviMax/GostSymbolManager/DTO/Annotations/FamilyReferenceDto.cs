using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.GostSymbolManager.DTO.Annotations
{
    public class FamilyReferenceDto
    {
        public string FamilyId { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string FamilyType { get; set; } = string.Empty;
        public override string ToString()
        {
            return $"FamilyReferenceDto: FamilyId: {FamilyId}, FamilyName: {FamilyName}, FamilyType: {FamilyType}";
        }
    }
}
