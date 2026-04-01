using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.GostSymbolManager.DTO.Annotations
{
    internal class SymbolColorDto
    {
        public Byte R { get; set; } = (Byte)0;
        public Byte G { get; set; } = (Byte)0;
        public Byte B { get; set; } = (Byte)0;
        public override string ToString()
        {
            return $"{R},{G},{B}";
        }
    }
}
