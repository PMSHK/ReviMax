using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.core.Elements;

namespace ReviMax.Core
{
    internal class ReflectionClassFinder
    {
        public static List<T> GetChildClasses<T>()
        {
            List<T> children = [];
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
            foreach (var type in types)
            {
                T instance = (T)Activator.CreateInstance(type);
                if (instance == null) { continue; }
                children.Add(instance);
            }
            return children;
        }

        public static List<T> GetChildForInterface<T>()
        {
            List<T> children = [];
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface);
            foreach (var type in types)
            {
                T instance = (T)Activator.CreateInstance(type);
                if (instance == null) { continue; }
                children.Add(instance);
            }
            return children;
        }

        public static List<T> GetChildClasses<T>(object[] parameter)
        {
            List<T> children = [];
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
            foreach (var type in types)
            {
                T instance = (T)Activator.CreateInstance(type, parameter);
                if (instance == null) { continue; }
                children.Add(instance);
            }
            return children;
        }
    }
}
