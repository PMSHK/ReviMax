using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Config;

namespace ReviMax.Core.Utils.Config
{
    class AssemblyResolver
    {
        public static void Register()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var assemblyName = new AssemblyName(args.Name).Name + ".dll";
                var pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var assemblyPath = Path.Combine(pluginFolder, assemblyName);
                ReviMaxLog.Information($"Assembly path: {assemblyPath}");
                return File.Exists(assemblyPath) ? Assembly.Load(assemblyPath) : null;
            };
        }
    }
}
