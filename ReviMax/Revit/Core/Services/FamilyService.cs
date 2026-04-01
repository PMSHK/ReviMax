using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ReviMax.Core.Config;
using ReviMax.Core.Utils;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Managers;
using ReviMax.GostSymbolManager.Models.Annotations;
using ReviMax.GostSymbolManager.UI.ViewModel;

namespace ReviMax.Revit.Core.Services
{
    internal class FamilyService
    {
        private Document Doc;
        private Dictionary<string, FamilyVM> Families { get; } = new Dictionary<string, FamilyVM>();
        public event Action FamilyLoaded;

        public FamilyService(Document doc)
        {
            Doc = doc;
        }

        public Dictionary<string, FamilyVM> FindAll()
        {

            string path = PathManager.GetFamiliesPath();
            var files = Directory.GetFiles(path, "*.rfa", SearchOption.TopDirectoryOnly);
            var familyLoader = new FamilyLoader();
            foreach (var f in files)
            {
                var family = familyLoader.Load(f);
                if (family != null && !Families.ContainsKey(family.Title))
                {
                    Families.Add(family.Title, family);
                }
            }
            return Families;
        }

        public bool IsFamilyLoaded(string familyName)
        {
            var loaded = new FilteredElementCollector(Doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Any(f => f.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase));
            Families.TryGetValue(familyName, out FamilyVM? family);
            return loaded;
        }

        public void LoadFamily(string familyName)
        {

            if (Families.TryGetValue(familyName, out FamilyVM? family) && !family.IsLoaded)
            {
                string path = PathManager.GetFamiliesPath();
                string filePath = Path.Combine(path, $"{familyName}.rfa");
                if (File.Exists(filePath))
                {
                    Doc.StartTransaction("Load Family", doc =>
                    {
                        bool loaded = doc.LoadFamily(filePath);
                        if (loaded) { family.IsLoaded = loaded; }
                    });

                }
            }

        }

        public void FillFamilyProperties(string familyName, FamilyReference reference)
        {
            if (!string.IsNullOrWhiteSpace(familyName))
            {
                Family? family = FindFamilyByName(familyName);
                if (family != null)
                {
                    FamilySymbol symbol = family.GetFamilySymbolIds()
                    .Select(id => Doc.GetElement(id))
                    .OfType<FamilySymbol>()
                    .FirstOrDefault();
                    reference.FamilyName = family.Name;
                    reference.FamilyId = family.Id.ToString();
                    reference.FamilyType = symbol?.Name ?? "";
                }
            }
        }

        public void LoadFamilyIfNotLoaded(List<string> familyNames)
        {
            ReviMaxLog.Information($"Checking if families are loaded: {string.Join(", ", familyNames)}");
            if (familyNames == null || familyNames.Count == 0) return;

            FindAll();
            foreach (string familyName in familyNames)
            {
                IsFamilyLoaded(familyName);
                LoadFamily(familyName);
                ReviMaxLog.Information($"Family '{familyName}' loaded");
            }
        }

        public Family? FindFamilyByName(string familyName)
        {
            Family family = new FilteredElementCollector(Doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .FirstOrDefault(f => f.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase));
            if (family != null) {return family; }
            ReviMaxLog.Warning($"Family '{familyName}' not found in the document.");
            return null;
        }

        public FamilySymbol? FindFamilySymbolByName(string familyName) 
        {
            Family? family = FindFamilyByName(familyName);
            if (family != null)
            {
                FamilySymbol symbol = family.GetFamilySymbolIds()
                    .Select(id => Doc.GetElement(id))
                    .OfType<FamilySymbol>()
                    .FirstOrDefault();
                if (symbol != null) { return symbol; }
                ReviMaxLog.Warning($"No symbols found for family '{familyName}'.");
                return null;
            }
            return null;
        }


    }
}
