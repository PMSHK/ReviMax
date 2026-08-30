using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.CircuitAnalyzeManager.Models.Enums;

namespace ReviMax.CircuitAnalyzeManager.Models
{
    class GraphCandidate
    {
        public ElementId ElementId { get; set; }

        public CandidateLocation LocationType { get; set; }

        public string? NodeId { get; set; }

        public string? EdgeId { get; set; }

        public XYZ ProjectionPoint { get; set; } = XYZ.Zero;

        public double Distance { get; set; }
    }
}
