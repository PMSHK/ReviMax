using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.CircuitAnalyzeManager.Models
{
    internal class EquipmentNode : IEquatable<EquipmentNode>
    {
        public ElementId Id { get; set; } = ElementId.InvalidElementId;
        public string Name { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            return obj is EquipmentNode node &&
                   Id == node.Id &&
                   Name == node.Name &&
                   UniqueId == node.UniqueId;
        }

        public bool Equals(EquipmentNode other)
        {
            return Id == other.Id &&
                   Name == other.Name &&
                   UniqueId == other.UniqueId;
        }

        public override int GetHashCode()
        {
            int hashCode = -296716303;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UniqueId);
            return hashCode;
        }
    }
}
