using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.CircuitAnalyzeManager.Models.Enums
{
    internal enum CircuitGraphNodeType
    {
        Unknown,

        Endpoint,
        Junction,
        Branch,

        Equipment,
        JunctionBox,

        Logical
    }
}
