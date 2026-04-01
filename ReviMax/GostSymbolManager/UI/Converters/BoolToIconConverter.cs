using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FontAwesome.WPF;

namespace ReviMax.GostSymbolManager.UI.Converters
{
    public class BoolToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLoaded)
                return isLoaded ? FontAwesomeIcon.Check : FontAwesomeIcon.Times;

            return FontAwesomeIcon.Times;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
