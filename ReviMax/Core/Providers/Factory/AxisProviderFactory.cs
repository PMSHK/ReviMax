using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Providers;
using ReviMax.Core.Utils.Managers;

namespace ReviMax.Core.Providers.Factory
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
            return ReflectionClassManager.GetClassesFromAssembly<AxisProvider>(new object[] { _doc });
        }
    }
}
