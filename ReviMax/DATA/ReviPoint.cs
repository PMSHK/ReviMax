using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.DATA
{
    public readonly struct ReviPoint
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public ReviPoint (double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
