using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Utils.Converter;
using ReviMax.GostSymbolManager.DTO.Annotations;
using ReviMax.GostSymbolManager.Models;
using ReviMax.GostSymbolManager.Models.Annotations;

namespace ReviMax.GostSymbolManager.Mapper
{
    internal static class CableSysSettingsMapper
    {
        public static CableSystemSettings ToModel(this CableSystemSettingsDto dto)
        {
            if (dto == null) return new CableSystemSettings();
            return new CableSystemSettings
            {
                GeneralSettings = dto.GeneralSettings !=null ? dto.GeneralSettings.ToModel() : new(),
                LineSettings = dto.LineSettings.Where(s=>s != null).Select(s=>s.ToModel()).ToList(),
            }; 
        }
        public static CableSystemSettingsDto ToDto(this CableSystemSettings model)
        {
            if (model == null) return new CableSystemSettingsDto();
            return new CableSystemSettingsDto
            {
                GeneralSettings = model.GeneralSettings != null ? model.GeneralSettings.ToDto() : new(),
                LineSettings = model.LineSettings.Where(s=> s != null).Select(s=>s.ToDto()).ToList(),
            };
        }
        private static BaseCableSysSettings ToModel(this BaseCableSysSettingsDto dto)
        {
            if (dto == null) return new BaseCableSysSettings();
            return new BaseCableSysSettings
            {
                Step = dto.StepMm.MillimetersToFeet(),
                Offset = dto.OffsetMm.MillimetersToFeet(),
                Snap = dto.SnapMm.MillimetersToFeet(),
                MinDist = dto.MinDistMm.MillimetersToFeet(),
                NodeClearance = dto.NodeClearanceMm.MillimetersToFeet(),
                CornerClearance = dto.CornerClearanceMm.MillimetersToFeet()
            };
        }
        private static BaseCableSysSettingsDto ToDto(this BaseCableSysSettings model)
        {
            if (model == null) return new BaseCableSysSettingsDto();
            return new BaseCableSysSettingsDto
            {
                StepMm = model.Step.FeetToMillimeters(),
                OffsetMm = model.Offset.FeetToMillimeters(),
                SnapMm = model.Snap.FeetToMillimeters(),
                MinDistMm = model.MinDist.FeetToMillimeters(),
                NodeClearanceMm = model.NodeClearance.FeetToMillimeters(),
                CornerClearanceMm = model.CornerClearance.FeetToMillimeters()
            };
        }
    }
}
