using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Core.Utils
{
    internal interface ILoad <T> where T : class
    {
        T? Load(string filePath);
    }
}
