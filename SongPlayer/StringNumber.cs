using System;
using System.Globalization;
using System.Windows.Data;

namespace SongPlayer
{
    [ValueConversion(typeof(int), typeof(string))]
    public class StringNumber : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? Settings.GetWord(Words.Rewritten) : ((int)value).ToString(); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == Settings.GetWord(Words.Rewritten) ? 0.ToString() : (string)value; 
        }
    }
}
