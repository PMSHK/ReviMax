using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.GostSymbolManager.Models.Annotations
{
    public class ReviWeights
    {
        List<Weight> Weights { get; }
        List<string> Scales;
        public ReviWeights()
        {
            Scales = new()
            { 
                "1:10","1:20","1:50","1:100","1:200","1:500"
            };
            Weights = new (16) 
            { 
                new Weight(1, new List<double> { 0.18, 0.18, 0.18, 0.1, 0.1, 0.1 }, Scales),
                new Weight(2, new List<double> { 0.25, 0.25, 0.25, 0.18, 0.1, 0.1 }, Scales),
                new Weight(3, new List<double> { 0.35, 0.35, 0.35, 0.25, 0.18, 0.1 }, Scales),
                new Weight(4, new List<double> { 0.7, 0.5, 0.5, 0.35, 0.25, 0.18 }, Scales),
                new Weight(5, new List<double> { 1.0, 0.7, 0.7, 0.5, 0.35, 0.5 }, Scales),
                new Weight(6, new List<double> { 1.4, 1.0, 1.0, 0.7, 0.2, 0.35 }, Scales),
                new Weight(7, new List<double> { 2.0, 1.4, 1.4, 1.0, 0.7, 0.5 }, Scales),
                new Weight(8, new List<double> { 2.8, 2.0, 2.0, 1.4, 1.0, 0.7 }, Scales),
                new Weight(9, new List<double> { 4.0, 2.8, 2.8, 2.0, 1.4, 1.0 }, Scales),
                new Weight(10, new List<double> { 5.0, 4.0, 4.0, 2.8, 2.0, 1.4 }, Scales),
                new Weight(11, new List<double> { 6.0, 5.0, 5.0, 4.0, 2.8, 2.8 }, Scales),
                new Weight(12, new List<double> { 7.0, 6.0, 6.0, 5.0, 4.0, 2.8 }, Scales),
                new Weight(13, new List<double> { 8.0, 7.0, 7.0, 6.0, 5.0, 4.0 }, Scales),
                new Weight(14, new List<double> { 9.0, 8.0, 8.0, 7.0, 6.0, 5.0 }, Scales),
                new Weight(15, new List<double> { 9.0, 9.0, 9.0, 8.0, 7.0, 6.0 }, Scales),
                new Weight(16, new List<double> { 9.0, 9.0, 9.0, 9.0, 8.0, 7.0 }, Scales)
            };
        }

        public void AddWeightForScale(List<double> weight, string scale)
        {
            if (weight.Count == 16 || !string.IsNullOrEmpty(scale))
            { 
                Scales.Add(scale);
                for (int i = 0; i < 16; i++)
                {
                    Weights[i].AddWeight(weight[i], scale);
                }
            }
        }

        public void RemoveWeightsForScale(string scale)
        {
            if (string.IsNullOrEmpty(scale) || !Scales.Contains(scale))
            {
                return;
            }
            int index = Scales.IndexOf(scale);
            for (int i = 0; i < 16; i++)
            {
                Weights[i].RemoveWeightsById(index);
            }
            Scales.RemoveAt(index);
        }
    }
}
