using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.CircuitAnalyzeManager.Models.Cables;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal class CircuitGraphEdge
    {
        public string Id { get; set; } = string.Empty;
        public string StartNodeId { get; set; } = string.Empty;
        public string EndNodeId { get; set; } = string.Empty;
        public List<string> CableIds { get; set; } = [];
        public EquipmentNode HostElement { get; set; } = new();
        //public ElementId? HostElementId { get; set; } // ID кабельного лотка или трубы, в которой проложены кабели
        //public string HostUniqueId { get; set; } = string.Empty; // UniqueID кабельного лотка или трубы, в которой проложены кабели
        public double Length { get ; set; } = double.MaxValue;
    }
}
