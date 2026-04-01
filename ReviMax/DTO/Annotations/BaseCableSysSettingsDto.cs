using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.DTO.Annotations
{
    internal class BaseCableSysSettingsDto
    {   
        public double StepMm { get; set; }
        public double OffsetMm { get; set; }
        public double SnapMm { get; set; }
        public double MinDistMm { get; set; }
        public double NodeClearanceMm { get; set; }
        public double CornerClearanceMm { get; set; }

        public override string ToString()
        {
            return $"BaseCableSysSettingsDto: StepMm: {StepMm}, OffsetMm: {OffsetMm}, SnapMm: {SnapMm}, MinDistMm: {MinDistMm}, NodeClearanceMm: {NodeClearanceMm}, CornerClearanceMm: {CornerClearanceMm}";
        }
    }

}
