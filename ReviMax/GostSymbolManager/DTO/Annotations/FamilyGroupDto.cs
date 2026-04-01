using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.GostSymbolManager.Models.Annotations;

namespace ReviMax.GostSymbolManager.DTO.Annotations
{
    public class FamilyGroupDto
    {
        public FamilyReferenceDto Family {  get; set; } = new FamilyReferenceDto();
        public FamilyMode FamilyMode { get; set; } = FamilyMode.TRAY;

        public override string ToString()
        {
            return $"FamilyGroupDto: Family: {Family}, FamilyMode: {FamilyMode}";
        }
    }
}
