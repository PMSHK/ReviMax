using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.CircuitAnalyzeManager.Models
{
    internal class RouteElementInfo : IEquatable<RouteElementInfo>
    {
        public ElementId Id { get; set; } = ElementId.InvalidElementId;
        public string Name { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;
        public double Section { get; set; } = 0;
        public double Length { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            return obj is RouteElementInfo node &&
                   Id == node.Id &&
                   Name == node.Name &&
                   UniqueId == node.UniqueId &&
                   Section == node.Section &&
                   Length == node.Length;
        }

        public bool Equals(RouteElementInfo other)
        {
            return Id == other.Id &&
                   Name == other.Name &&
                   UniqueId == other.UniqueId &&
                   Section == other.Section &&
                   Length == other.Length;
        }

        public override int GetHashCode()
        {
            int hashCode = -296716303;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UniqueId);
            hashCode = hashCode * -1521134295 + Section.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            return hashCode;
        }
    }
}
