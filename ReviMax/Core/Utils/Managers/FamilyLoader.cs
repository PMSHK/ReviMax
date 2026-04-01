using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Config;
using ReviMax.GostSymbolManager.UI.ViewModel;

namespace ReviMax.Core.Utils.Managers
{
    internal class FamilyLoader : ILoad<FamilyVM>
    {
        public FamilyVM? Load(string filePath)
        {
            try {
                if (File.Exists(filePath))
                {
                    var name = Path.GetFileNameWithoutExtension(filePath);
                    var familyVM = new FamilyVM()
                    {
                        Title = name,
                        IsLoaded = false
                    };
                    return familyVM;
                }
                return null;
            } catch (Exception ex) {
              ReviMaxLog.Error($"Failed to load family from {filePath}", ex);
              throw new FileNotFoundException("Failed to load family", ex);
            }
        }
    }
}
