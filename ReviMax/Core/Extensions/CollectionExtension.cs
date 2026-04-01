using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Core.Extensions
{
    public static class CollectionExtension
    {
        public static List<T> CombineAll<T>(params IEnumerable<T>[] lists)
        {
            List<T> result = new List<T>();
            foreach (var list in lists)
            {
                if (list != null)
                {
                    result.AddRange(list);
                }
            }
            return result;
        }

    }
}
