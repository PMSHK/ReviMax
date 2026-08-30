using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Core.Enums;

namespace ReviMax.Core.Filters
{
    internal class AllElectricalEquipmentFilter : ICableSystemCategory
    {
        public IEnumerable<BuiltInCategory> GetCategory()
        {
            return
                [
                    //BuiltInCategory.OST_CableTray,
                    //BuiltInCategory.OST_Conduit,
                    //BuiltInCategory.OST_CableTrayFitting,
                    //BuiltInCategory.OST_ConduitFitting,

                    BuiltInCategory.OST_ElectricalEquipment,
                    BuiltInCategory.OST_ElectricalFixtures,

                    BuiltInCategory.OST_DataDevices,
                    BuiltInCategory.OST_CommunicationDevices,
                    BuiltInCategory.OST_FireAlarmDevices,
                    BuiltInCategory.OST_SecurityDevices,
                    BuiltInCategory.OST_TelephoneDevices,

                    BuiltInCategory.OST_LightingFixtures,
                    BuiltInCategory.OST_LightingDevices,

                    BuiltInCategory.OST_MechanicalEquipment,
                    BuiltInCategory.OST_SpecialityEquipment,

                    BuiltInCategory.OST_GenericModel
                ];
        }

        public Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> GetGroupedCategories()
        {
            return new Dictionary<FamilyMode, IEnumerable<BuiltInCategory>> 
            {
                //[FamilyMode.TRAY] = [BuiltInCategory.OST_CableTray, BuiltInCategory.OST_CableTrayFitting],
                //[FamilyMode.CONDUITS] = [BuiltInCategory.OST_Conduit, BuiltInCategory.OST_ConduitFitting],
                [FamilyMode.ELECTRICAL_EQUIPMENT] = [BuiltInCategory.OST_ElectricalEquipment],
                [FamilyMode.ELECTRICAL_FIXTURES] = [BuiltInCategory.OST_ElectricalFixtures],
                [FamilyMode.LIGHTNING_EQUIPMENT] = [BuiltInCategory.OST_LightingDevices],
                [FamilyMode.LIGHTNING_FIXTURES] = [BuiltInCategory.OST_LightingFixtures],
                [FamilyMode.SIGNL_EQUIPMENT] = [
                    BuiltInCategory.OST_DataDevices,
                    BuiltInCategory.OST_CommunicationDevices,
                    BuiltInCategory.OST_FireAlarmDevices,
                    BuiltInCategory.OST_SecurityDevices,
                    BuiltInCategory.OST_TelephoneDevices,],
                [FamilyMode.GENERAL_EQUIPMENT] = [
                    BuiltInCategory.OST_MechanicalEquipment,
                    BuiltInCategory.OST_SpecialityEquipment,

                    BuiltInCategory.OST_GenericModel],

            };
        }

        public bool CanHandle(HashSet<BuiltInCategory> categories)
        {
            //bool hasTray = categories.Contains(BuiltInCategory.OST_CableTray) ||
            //               categories.Contains(BuiltInCategory.OST_CableTrayFitting);

            //bool hasConduit = categories.Contains(BuiltInCategory.OST_Conduit) ||
            //                  categories.Contains(BuiltInCategory.OST_ConduitFitting);


            bool hasElectricalEquipment = categories.Contains(BuiltInCategory.OST_ElectricalEquipment);
            bool hasElectricalFixtures = categories.Contains(BuiltInCategory.OST_ElectricalFixtures);
            bool hasLightningEquipment = categories.Contains(BuiltInCategory.OST_LightingDevices);
            bool hasLightningFixtures = categories.Contains(BuiltInCategory.OST_LightingFixtures);
            bool hasSignalEquipment = categories.Contains(BuiltInCategory.OST_DataDevices) ||
                                 categories.Contains(BuiltInCategory.OST_CommunicationDevices) ||
                                 categories.Contains(BuiltInCategory.OST_FireAlarmDevices) ||
                                 categories.Contains(BuiltInCategory.OST_SecurityDevices) ||
                                 categories.Contains(BuiltInCategory.OST_TelephoneDevices);
            bool hasGeneralEquipment = categories.Contains(BuiltInCategory.OST_MechanicalEquipment) ||
                                categories.Contains(BuiltInCategory.OST_SpecialityEquipment) ||
                                categories.Contains(BuiltInCategory.OST_GenericModel);
            return 
                //hasTray 
                //&& hasConduit 
                //&& 
                hasElectricalEquipment 
                && hasElectricalFixtures 
                && hasLightningEquipment 
                && hasLightningFixtures 
                && hasSignalEquipment 
                && hasGeneralEquipment;
        }

    }
}
