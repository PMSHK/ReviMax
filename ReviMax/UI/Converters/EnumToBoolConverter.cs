using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace ReviMax.UI.Converters
{
    public sealed class EnumToBoolConverter : IValueConverter
    {
        // Enum -> bool (поставить галочку)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;

            var enumValue = value.ToString();
            var targetValue = parameter.ToString();

            return string.Equals(enumValue, targetValue, StringComparison.Ordinal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return Binding.DoNothing;

            // ConvertBack вызывается и на false тоже, нам нужно реагировать только на true
            if (value is bool b && b)
            {
                // targetType будет CableSystemFilterMode
                return Enum.Parse(targetType, parameter.ToString()!);
            }

            return Binding.DoNothing;
        }
    }
}

