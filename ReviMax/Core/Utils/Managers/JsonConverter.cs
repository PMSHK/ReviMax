using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Config;
using Newtonsoft.Json;


namespace ReviMax.Core.Utils.Managers
{
    internal class JsonConverter
    {
        public static bool Deserialize<T>(string json, out T? result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
                return result !=null;
            }
            catch (Exception ex)
            {
                ReviMaxLog.Warning($"Failed to deserialize JSON: {ex.Message}");
                result = default;
                return false;
            }
        }

        public static string Serialize<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            catch (Exception ex)
            {
                ReviMaxLog.Warning($"Failed to serialize object to JSON: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
