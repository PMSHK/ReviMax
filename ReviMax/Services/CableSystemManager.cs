using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using ReviMax.Calculators;
using ReviMax.Config;
using ReviMax.DATA;

namespace ReviMax.Services
{
    public class CableSystemManager
    {
        public Document Doc { get; }
        public CableSystemManager(Document doc) { Doc = doc; }

        public Curve GetAxisProjection (CableTrayConduitBase cableSystem)
        {
            Curve axis = GetCurve(cableSystem);

            View view = Doc.ActiveView;

            double z = view.Origin.Z;
            XYZ p0 = axis.GetEndPoint(0);
            XYZ p1 = axis.GetEndPoint(1);
            XYZ dir = (p1 - p0).Normalize();

            if (Math.Abs(dir.Z) > 0.99)
                return null;

            XYZ p0Projected = new XYZ(p0.X, p0.Y, z);
            XYZ p1Projected = new XYZ(p1.X, p1.Y, z);
            Curve projectedAxes = Line.CreateBound(p0Projected, p1Projected);
            
            return projectedAxes;
        }

        public IList<Curve> GetFittingPojectionLine(FamilyInstance fitting)
        {
            if (fitting == null)
            {
                throw new ArgumentNullException(nameof(fitting), "Fitting cannot be null.");
            }
            if (fitting.MEPModel == null)
            {
                ReviMaxLog.Warning(
                    $"Fitting {fitting.Id} has no MEPModel. Skipping.");
                return new List<Curve>();
            }
            var connectorManager = fitting.MEPModel.ConnectorManager;
            if (connectorManager == null)
            {
                ReviMaxLog.Warning(
                    $"Fitting {fitting.Id} has no ConnectorManager. Skipping.");
                return new List<Curve>();
            }
            var connectors = connectorManager.Connectors;
            if (connectors.Size == 0)
            {
                ReviMaxLog.Warning(
                    $"Fitting {fitting.Id} has no Connectors. Skipping.");
                return new List<Curve>();
            }
            var points = new List<ReviPoint>();
            double z = Doc.ActiveView.Origin.Z;
            foreach (Connector connector in connectors) {
                if (connector == null) continue;

                XYZ point = connector.Origin;
                    points.Add(new (point.X,point.Y,z));
            }

            if (points.Count < 2)
            {
                ReviMaxLog.Warning(
                    $"Fitting {fitting.Id} has less than 2 connector points. Skipping.");
                return new List<Curve>();
            }

            ReviPoint midPoint = points.FindMidPoint();
            List<Curve> curves = new List<Curve>();
            foreach (ReviPoint point in points) 
            { 
                curves.Add(
                    Line.CreateBound(
                    new XYZ(point.X, point.Y, z),
                    new XYZ(midPoint.X, midPoint.Y, z)
                ));
            }
            return curves;
        }

        private Curve GetCurve (CableTrayConduitBase cableSystem)
        {
         return ((LocationCurve)cableSystem.Location).Curve;
        }


    }

     
    }
