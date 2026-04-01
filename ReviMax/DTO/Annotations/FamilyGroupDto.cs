using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Elements.Filters;
using ReviMax.Models.Annotations;

namespace ReviMax.DTO.Annotations
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
