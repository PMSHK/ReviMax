using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Models.Annotations
{
    public class Weight
    {
        public byte Id { get; }
        public List<double> Weights { get; set; }
        public List<string> Scales { get; set; }

        public Weight(byte id, List<double> weights, List<string> scales)
        {
            Id = id;
            Weights = weights;
            Scales = scales ?? [];
        }

        public void AddWeight(double weight, string scale)
        { if (weight >= 0 && (scale != null) && (scale.Length > 0))
            {
                Weights.Add(weight);
                Scales.Add(scale);
            }
        }
        public void RemoveWeightsForScale(string scale) {
            int index = Scales.IndexOf(scale);
            if (index >= 0)
            {
                Weights.RemoveAt(index);
                Scales.RemoveAt(index);
            }
        }

        public void RemoveWeightsById(int index) { 
            
            if (index >= 0 && index <= Weights.Count)
            {
                Weights.RemoveAt(index);
            }
        }

    }
}
