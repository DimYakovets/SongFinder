using System;
using System.Globalization;
using System.Windows.Data;

namespace SongPlayer
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
