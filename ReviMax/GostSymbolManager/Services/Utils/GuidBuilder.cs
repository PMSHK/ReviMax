using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.GostSymbolManager.Services.Utils
{
    internal class GuidBuilder
    {
        public static string CreateGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
