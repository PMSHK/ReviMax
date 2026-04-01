//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Autodesk.Revit.DB;
//using ReviMax.Models.Annotations;

//namespace ReviMax.Core.Elements.Filters
//{
//    internal class SelectedCableSystemCategory : ICableSystemCategory
//    {
//        private IEnumerable<BuiltInCategory>? _categories;
//        public IEnumerable<BuiltInCategory> Categories 
//        {
//            get => _categories?? [BuiltInCategory.OST_CableTray];
//            set => _categories = value?.ToArray() ?? [BuiltInCategory.OST_CableTray];
//        }
//        public IEnumerable<BuiltInCategory> GetCategory()
//        {
//            return Categories;
//        }

//        public Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> GetGroupedCategories()
//        {
//            throw new NotImplementedException();
//        }
//        public bool CanHandle(HashSet<BuiltInCategory> elements)
//        {
//            return true;
//        }
//    }
//}
