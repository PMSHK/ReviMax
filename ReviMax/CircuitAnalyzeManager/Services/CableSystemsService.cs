using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Enums;
using ReviMax.Core.Filters;
using ReviMax.Handlers;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Model;

namespace ReviMax.CircuitAnalyzeManager.Services
{
    internal class CableSystemsService
    {
        private Document _doc;
        public Document Doc { get => _doc; }
        public CableSystemsService(Document doc) { _doc = doc; }
        public ResultHandler<Dictionary<FamilyMode, IList<Element>>> GetAllCableSystems()
        {
            RevitFilterManager filterManager = new RevitFilterManager(Doc);
            var cableSystems = filterManager.GetCableElementsAll(new AllCableFilter());
            if (cableSystems == null) return ResultHandler<Dictionary<FamilyMode, IList<Element>>>.Failure("No cable systems found in the document.");
            return ResultHandler<Dictionary<FamilyMode, IList<Element>>>.Success(cableSystems);
        }

        public ResultHandler<Dictionary<FamilyMode, IList<Element>>> GetStraightCableSystems()
        {
            RevitFilterManager filterManager = new RevitFilterManager(Doc);
            var cableSystems = filterManager.GetCableElementsAll(new CableTraysConduitsFilter());
            if (cableSystems == null) return ResultHandler<Dictionary<FamilyMode, IList<Element>>>.Failure("No straight cable systems found in the document.");
            return ResultHandler<Dictionary<FamilyMode, IList<Element>>>.Success(cableSystems);
        }
    }
}
