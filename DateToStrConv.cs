using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cracker
{
    public class DateToStrConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            
            string format = "dd.MM.yyyy";
            if (parameter != null && (parameter.ToString().ToLower() == "datetime" || parameter.ToString().ToLower() == "time"))
            {
                format = "dd.MM.yyyy HH:mm";
            }
            
            return ((DateTime)value).ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt;
            return DateTime.TryParse((string)value, out dt) ? dt : DependencyProperty.UnsetValue;
        }
    }
}
