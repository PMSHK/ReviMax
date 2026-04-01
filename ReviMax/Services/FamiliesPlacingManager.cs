using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Config;   

namespace ReviMax.Services
{
    internal class FamiliesPlacingManager
    {
        private Document _doc;
        private View _activeView;
        public Document Doc { get => _doc; set => _doc = value; }
        public FamiliesPlacingManager(Document doc)
        {
            if (doc == null)
            {
                ReviMaxLog.Warning($"FamiliesPlacingManager. Document {nameof(doc)} cannot be null");
                throw new ArgumentNullException(nameof(doc), "Document cannot be null.");
            }
            _doc = doc;
            _activeView = _doc.GetActiveView();
        }

        public void PlaceFamilyInstance(FamilySymbol familySymbol, XYZ location, XYZ orientation)
        {
            if (familySymbol == null)
            {
                ReviMaxLog.Warning("FamiliesPlacingManager. FamilySymbol cannot be null.");
                throw new ArgumentNullException(nameof(familySymbol), "FamilySymbol cannot be null.");
            }
            if (!familySymbol.IsActive)
            {
                familySymbol.Activate();
                _doc.Regenerate();
            }
            FamilyInstance familyInstance = _doc.Create.NewFamilyInstance(location, familySymbol, _activeView);
            ReviMaxLog.Information($"FamiliesPlacingManager. Placed FamilyInstance of {familySymbol.Name} at location {location}.");
            // Optionally set orientation if needed
            if (orientation != null)
            {
                Line rotationAxis = Line.CreateBound(location, location + XYZ.BasisZ);
                double angle = orientation.AngleTo(XYZ.BasisX);
                ElementTransformUtils.RotateElement(_doc, familyInstance.Id, rotationAxis, angle);
                ReviMaxLog.Information($"FamiliesPlacingManager. Rotated FamilyInstance of {familySymbol.Name} by angle {angle} radians.");
            }
        }

        public IEnumerable<XYZ> GetInsertionPointAlongCurve(IList<Curve> curves, double step, double offset)
        {
            double stepMM = step / 304.8;
            double offsetMM = offset / 304.8;
            double accumulatedLength = 0.0;
            double target = stepMM;
            Curve _lastCurve = curves[0];
            foreach (Curve curve in curves)
            {
                double curveLength = curve.Length;
                while (accumulatedLength + curveLength >= target)
                {
                    double localCurvePosition = target - accumulatedLength;
                    double param = localCurvePosition / curveLength;
                    XYZ point = curve.Evaluate(param, true);
                    XYZ tangent = curve.ComputeDerivatives(param, true).BasisX.Normalize();
                    XYZ viewDirection = _activeView.ViewDirection.Normalize();
                    if (Math.Abs(tangent.DotProduct(viewDirection)) < 0.999) 
                    { 
                        XYZ normal = _activeView.ViewDirection.CrossProduct(tangent).Normalize();

                        target += stepMM;
                        ReviMaxLog.Information($"Point is: {point}, Tangent is: {tangent}, normal is: {normal}");
                        yield return point + offsetMM * normal;
                    }
                }
                accumulatedLength += curveLength;
                _lastCurve = curve;
            }
            yield return _lastCurve.GetEndPoint(1);
        }
    }
}
