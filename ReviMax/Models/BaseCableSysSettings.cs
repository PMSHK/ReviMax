using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Utils.Converter;


namespace ReviMax.Models
{
    public class BaseCableSysSettings
    {
        private double _step = 0;
        private double _offset = 0;
        private double _snap = 0;
        private double _minDist = 0;
        private double _nodeClearance = 0;
        private double _cornerClearance = 0;

        public BaseCableSysSettings() { }
        public BaseCableSysSettings(double step, double offset, double snap, double minDist, double nodeClearance, double cornerClearance)
        {
            _step = step;
            _offset = offset;
            _snap = snap;
            _minDist = minDist;
            _nodeClearance = nodeClearance;
            _cornerClearance = cornerClearance;
        }

        public double Step { get => _step; set => _step = value; }
        public double Offset { get => _offset; set => _offset = value; }
        public double Snap { get => _snap; set => _snap = value; }
        public double MinDist { get => _minDist; set => _minDist = value; }
        public double NodeClearance { get => _nodeClearance; set => _nodeClearance = value; }
        public double CornerClearance { get => _cornerClearance; set => _cornerClearance = value; }

        public override string ToString()
        {
            return $"Step: {_step}, Offset: {_offset}, Snap: {_snap}, MinDist: {_minDist}, NodeClearance: {_nodeClearance}, CornerClearance: {_cornerClearance}";
        }

        public bool Filled()
        {
            return _step > 0 && _offset >= 0 && _snap >= 0 && _minDist >= 0 && _nodeClearance >= 0 && _cornerClearance >= 0;
        }
        public void CoppyFrom(BaseCableSysSettings source)
        {
            if (ReferenceEquals(this, source)) return;
            if (source != null)
            {
                Step = source.Step;
                Offset = source.Offset;
                Snap = source.Snap;
                MinDist = source.MinDist;
                NodeClearance = source.NodeClearance;
                CornerClearance = source.CornerClearance;
            }
        }
    }
}
