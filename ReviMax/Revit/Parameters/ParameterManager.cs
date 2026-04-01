using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Config;

namespace ReviMax.Revit.Parameters
{
    internal static class ParameterManager
    {
        public static void SetParameterValue(this Element element, string paramName, string value)
        {
            Parameter param = element.LookupParameter(paramName);
            if (param != null && !param.IsReadOnly)
            {
                ReviMaxLog.Error($"{paramName} FOUND. IsReadOnly={param.IsReadOnly}, StorageType={param.StorageType}, HasValue={param.HasValue}");
                param.Set(value);
            }
            else
            {
                ReviMaxLog.Error($"{paramName} NOT FOUND. Element category: {element.Category?.Name}, class: {element.GetType().Name}");
                throw new InvalidOperationException($"Parameter {paramName} not found or is read-only.");
            }
        }

        public static void SetRunID(this Element element, string runID) {
            element.SetParameterValue("RM_RunId", runID);
        }

        public static void SetSourceElementID(this Element element, int sourceElementID) {
            element.SetParameterValue("RM_SourceElementId", sourceElementID.ToString());
        }
        public static void SetViewID(this Element element, int viewID) {
            element.SetParameterValue("RM_ViewId", viewID.ToString());
        }
        public static void SetType(this Element element, string type) {
            element.SetParameterValue("RM_Type", type);
        }
        public static void SetType(this Element element, string parameterName, string type)
        {
            element.SetParameterValue(parameterName, type);
        }

        public static Parameter GetParameter(Element element, string parameterName)
        {
            Parameter param = element.LookupParameter(parameterName);
            if (param == null) throw new InvalidOperationException($"Параметр {parameterName} не найден у элементов категории {element.Category}.");

            return param;
        }
    }
}
