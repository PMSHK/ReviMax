using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.GostSymbolManager.Providers;

namespace ReviMax.GostSymbolManager.Providers.Factory
{
    internal class AxisProviderFactory
    {
        private List<AxisProvider> _axisProviders;
        private  Document _doc;
        public AxisProviderFactory(Document doc)
        {
            _doc = doc;
            _axisProviders = GetCadAppClasses();
        }

        public AxisProvider GetProvider(Element element) 
        {
            return _axisProviders.FirstOrDefault(provider => provider.CanHandle(element));
        }
        public List<AxisProvider> GetCadAppClasses()
        {
            List<AxisProvider> axisProviders = [];
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AxisProvider)) && !t.IsAbstract);
            foreach (var type in types)
            {
                AxisProvider instance = (AxisProvider)Activator.CreateInstance(type, new object[] { _doc });
                if (instance == null) { continue; }
                axisProviders.Add(instance);
            }
            return axisProviders;
        }
    }
}
