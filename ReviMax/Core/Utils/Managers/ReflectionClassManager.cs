using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Config;
namespace ReviMax.Core.Utils.Managers
{
    internal class ReflectionClassManager
    {
        public static List<T> GetClassesFromAssembly<T>(params object[] constructorParams)
        {
            List<T> instances = [];
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
            foreach (var type in types)
            {
                T instance = (T)Activator.CreateInstance(type, constructorParams);
                if (instance == null) { continue; }
                instances.Add(instance);
            }
            ReviMaxLog.Information($"[ReflectionClassManager] Found {instances.Count} classes of type {typeof(T).Name} in assembly.");
            return instances;
        }

    }
}
