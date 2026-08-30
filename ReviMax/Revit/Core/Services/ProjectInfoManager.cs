using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Revit.Core.Services
{
    internal static class ProjectInfoManager
    {
        public static string GetProjectNumber(this Document doc)
        {
            if (doc == null) return string.Empty;
            return doc.ProjectInformation.Number;
        }

        public static string GetDocumentTitle(this Document doc)
        {
            if (doc == null) return string.Empty;
            return doc.Title;
        }

        public static string GetDocumentUniqueId(this Document doc)
        {
            if (doc == null) return string.Empty;
            return doc.ProjectInformation.UniqueId.Trim();
        }

        public static string GetDocumentIdentificationString(this Document doc)
        {
            if (doc == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"{GetDocumentTitle(doc)}: ").Append(GetDocumentUniqueId(doc)).Append($" | {GetProjectNumber(doc)}");
            return sb.ToString();
        }
    }
}
