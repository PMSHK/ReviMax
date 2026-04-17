using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ReviMax.Revit.Core.Services
{
    public class RevitParametersManager
    {
        public RevitParametersManager() { }
        public RevitParametersManager(string name) { }

        public void SetFamilyParameter(FamilyInstance instance, string paramName,double parameterValue)
        {
            Parameter p = instance.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
                p.Set(parameterValue);
        }

        public void SetFamilyParameter(FamilyInstance instance, string paramName, int parameterValue)
        {
            Parameter p = instance.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
                p.Set(parameterValue);
        }

        public void SetFamilyParameter(FamilyInstance instance, string paramName, string parameterValue)
        {
            Parameter p = instance.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
                p.Set(parameterValue);
        }

        public void SetFamilyParameter(FamilyInstance instance, string paramName, ElementId parameterValue)
        {
            Parameter p = instance.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
                p.Set(parameterValue);
        }

        public List<string> GetAllParameters(FamilyInstance instance)
        {
            return instance.Parameters
                .Cast<Parameter>()
                .Select(p=>p.Definition.Name)
                .ToList();
        }
    }
}
