using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Models.Annotations;

namespace ReviMax.GostSymbolManager.Mapper
{
    public class ColorMapper
    {
        public static Color FromSymbolColor(SymbolColor color)
        {
            return new Color(color.R, color.G, color.B);
        }

        public static SymbolColor ToSymbolColor(Color color)
        {
            return new SymbolColor(color.Red, color.Green, color.Blue);
        }
    }
}
