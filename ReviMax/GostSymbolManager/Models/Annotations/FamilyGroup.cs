using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.GostSymbolManager.Models.Annotations
{
    public class FamilyGroup
    {
        public FamilyReference Family { get; set; } = new FamilyReference();
        public FamilyMode FamilyMode { get; set; } = FamilyMode.TRAY;

        public FamilyGroup() { }
        public FamilyGroup(FamilyReference? family = null, FamilyMode? familyMode = FamilyMode.TRAY)
        {
            Family = family??new();
            FamilyMode = familyMode??FamilyMode.TRAY;
        }
        public bool Filled()
        {
            return Family.Filled();
        }

        public override string ToString()
        {
            return $"{Family.ToString()}, FamilyMode: {FamilyMode}";
        }
    }
}
