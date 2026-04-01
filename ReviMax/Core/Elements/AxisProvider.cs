using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Models.Revit;
using ReviMax.Services;

namespace ReviMax.core.Elements
{
    internal abstract class AxisProvider
    {
        private Document _document;
        private CableSystemManager _cableSystemManager;
        public Document Doc { get => _document; }
        public CableSystemManager CableSystemManager { get => _cableSystemManager; }
        public AxisProvider(Document document)
        {
            _document = document;
            _cableSystemManager = new CableSystemManager(document);
        }
        public abstract IEnumerable<AxisSegment> GextAxes(Element element, View view);
        public abstract bool CanHandle(Element element);
        public virtual bool IsValidCurve(Curve curve)
        {
            return curve != null && curve.IsBound && curve.Length > _document.Application.ShortCurveTolerance;
        }
    }
}
