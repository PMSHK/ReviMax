using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ReviMax.Core.Config;

namespace ReviMax.Core.Utils.Managers
{
    internal class TextLoader : ILoad<string>
    {
        public string Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                ReviMaxLog.Information($"Loading text file: {filePath}");
                return File.ReadAllText(filePath);
            }
            ReviMaxLog.Warning($"Text file not found: {filePath}");
            return string.Empty;
        }
    }
}
