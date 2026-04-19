using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;

namespace ReviMax.Revit.Core.Services
{
    public class RevitParametersManager
    {
        public RevitParametersManager() { }
        public RevitParametersManager(string name) { }

        public void SetInstanceParameter(FamilyInstance instance, string paramName,double parameterValue)
        {
            Parameter p = instance.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
                p.Set(parameterValue);
        }

        public void SetFamilyParameter(Element family, string paramName, object parameterValue)
        {
            if (parameterValue == null) return;
            Parameter p = family.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
                switch (parameterValue) 
                {
                    case int intValue:
                        p.Set(intValue);
                        ReviMaxLog.Information($"Set parameter {paramName} to int value {intValue} in family {family.Name}");
                        break;
                    case double doubleValue:
                        p.Set(doubleValue);
                        ReviMaxLog.Information($"Set parameter {paramName} to double value {doubleValue} in family {family.Name}");
                        break;
                    case string stringValue:
                        p.Set(stringValue);
                        ReviMaxLog.Information($"Set parameter {paramName} to string value {stringValue} in family {family.Name}");
                        break;
                    case ElementId elementIdValue:
                        p.Set(elementIdValue);
                        ReviMaxLog.Information($"Set parameter {paramName} to ElementId value {elementIdValue} in family {family.Name}");
                        break;
                    default:
                        throw new ArgumentException($"Unsupported parameter type: {parameterValue.GetType()}");
                }
            ;
            
        }

        public string LookUpParameterValue(FamilyInstance instance, string paramName)
        {
            Parameter p = instance.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
            {
                ReviMaxLog.Information($"Parameter {paramName} found with value {p.AsValueString()}");
                return p.AsValueString();
            }

            else
                ReviMaxLog.Warning($"Parameter {paramName} not found in family instance {instance.Id}");
            return string.Empty;
        }
        public string LookUpParameterValue(Element element, string paramName)
        {
            Parameter p = element.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
            {
                ReviMaxLog.Information($"Parameter {paramName} found with value {p.AsValueString()}");
                return p.AsValueString();
            }

            else
                ReviMaxLog.Warning($"Parameter {paramName} not found in family {element.Id}");
            return string.Empty;
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
        public List<string> GetAllParameters(Element element)
        {
            return element.Parameters
                .Cast<Parameter>()
                .Select(p => p.Definition.Name)
                .ToList();
        }

        public List<string> GetAllEditableParameters(Element element)
        {
            return element.Parameters
                .Cast<Parameter>()
                .Where(p => !p.IsReadOnly)
                .Select(p => p.Definition.Name)
                .ToList();
        }
    }
}
