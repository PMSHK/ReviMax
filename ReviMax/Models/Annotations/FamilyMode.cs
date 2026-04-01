using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Models.Annotations
{
    public enum FamilyMode
    {
        [Description("Лоток")]
        TRAY,

        [Description("Труба")]
        CONDUITS
    }
}
