using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Utils.Config;
using System.Numerics;

namespace ReviMax.Core.Utils.Converter
{
    internal static class DimensionConverter
    {
        public static double FeetToMillimeters(this object feet) 
        {
            if (feet == null) return 0;
            return Convert(feet) * 304.8;
        }
        public static double MillimetersToFeet(this object millimeters)
        {
            return Convert(millimeters) / 304.8;
        }

        private static double Convert (object value)
        {
            return value switch
            {
                int intValue => (double)intValue,
                double doubleValue => doubleValue,
                byte byteValue => (double)byteValue,
                decimal decimalValue => (double)decimalValue,
                float floatValue => (double)floatValue,
                long longValue => (double)longValue,
                string stringValue when double.TryParse(stringValue, out double result) => result,
                _ => 0
            };
        }


    }
}
