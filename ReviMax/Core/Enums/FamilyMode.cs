using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Core.Enums
{
    public enum FamilyMode
    {
        [Description("Лоток")]
        TRAY,

        [Description("Труба")]
        CONDUITS,

            
        STRAIGHT_CABLE_SYSTEMS,

        [Description("Силовое оборудование")]
        ELECTRICAL_EQUIPMENT,

        [Description("Электроустановочные изделия")]
        ELECTRICAL_FIXTURES,

        [Description("Осветительные приборы")]
        LIGHTNING_EQUIPMENT,

        [Description("Устройства освещения")]
        LIGHTNING_FIXTURES,
        
        [Description("Слаботочное оборудование")]
        SIGNL_EQUIPMENT,

        [Description("Механическое оборудование")]
        GENERAL_EQUIPMENT,
    }
}
