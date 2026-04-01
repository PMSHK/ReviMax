using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.DTO
{
    internal class ReviElement
    {
        private int _runID = 0;
        private int _elementID = 0;
        private string _type = "";
        private int _viewID = 0;
        public int RunID { get => _runID; set => _runID = value; }
        public int ElementID { get => _elementID; set => _elementID = value; }
        public string Type { get => _type; set => _type = value; }
        public int ViewID { get => _viewID; set => _viewID = value; }

        public ReviElement() { }
        public ReviElement(int runID, int elementID, string type, int viewID)
        {
            _runID = runID;
            _elementID = elementID;
            _type = type;
            _viewID = viewID;
        }
    }
}
