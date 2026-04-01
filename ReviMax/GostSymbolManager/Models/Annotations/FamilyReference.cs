using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.GostSymbolManager.Models.Annotations
{
    public class FamilyReference
    {
        public string FamilyId { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string FamilyType { get; set; } = string.Empty;

        public bool Filled() 
        {  
            return FamilyId != string.Empty && FamilyName!=string.Empty && FamilyType!=string.Empty; 
        }

        public override string ToString()
        {
            return $"FamilyId: {FamilyId}, FamilyName: {FamilyName}, FamilyType: {FamilyType}";
        }
    }
}
