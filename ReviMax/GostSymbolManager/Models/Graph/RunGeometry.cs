using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.GostSymbolManager.Models.Graph
{
    internal class RunGeometry
    {
        public GraphRun Run { get; set; } = null!;
        public XYZ Start { get; set; }
        public XYZ End { get; set; }
        public XYZ Dir { get; set; }
        public double Length { get; set; }
        public XYZ Mid => (Start + End) * 0.5;
    }
}
