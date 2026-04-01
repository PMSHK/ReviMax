using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using ReviMax.Core.Utils.Config;

namespace ReviMax.Revit.Parameters
{
    internal class ParameterBinder
    {
        public static void EnsureSharedParameters(Document doc)
        {
            Application app = doc.Application;

            string spPath = PathManager.GetFilePathInDirectory(PathManager.GetAppDataPath(), "ReviMaxParameters.txt");
            if (!File.Exists(spPath))
                throw new InvalidOperationException("Shared parameter file not found");

            app.SharedParametersFilename = spPath;

            DefinitionFile defFile = app.OpenSharedParameterFile();
            DefinitionGroup group = defFile.Groups.get_Item("ReviMax");

            var detailComponents = app.Create.NewCategorySet();
            detailComponents.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_DetailComponents));

            BindTextParam(app, doc, group, "RM_DC_COLOR", detailComponents);
        }

        private static void BindTextParam(
            Application app,
            Document doc,
            DefinitionGroup group,
            string name,
            CategorySet categories)
        {
            Definition def = group.Definitions.get_Item(name);
            if (def == null)
                throw new InvalidOperationException($"Shared parameter {name} not found");

            InstanceBinding binding = app.Create.NewInstanceBinding(categories);

            BindingMap map = doc.ParameterBindings;
            if (!map.Contains(def))
            {
                using (Transaction t = new Transaction(doc, $"Bind {name}"))
                {
                    t.Start();
                    map.Insert(def, binding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                    t.Commit();
                }
            }
        }
    }
}
