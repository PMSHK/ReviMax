using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Models.Annotations
{
    public class SymbolColor
    {
        public Byte R { get; set; } = (Byte)0;
        public Byte G { get; set; } = (Byte)0;
        public Byte B { get; set; } = (Byte)0;
        public SymbolColor() { }
        public SymbolColor(Byte r, Byte g, Byte b)
        {
            if (r > 255 || g > 255 || b > 255 || r < 0 || g < 0 || b < 0)
            {
                throw new ArgumentOutOfRangeException("Color components must be between 0 and 255.");
            }
            R = r;
            G = g;
            B = b;
        }
        public override string ToString()
        {
            var sb = new StringBuilder(R + "," + G + "," + B);
            return sb.ToString();
        }
    }
}
