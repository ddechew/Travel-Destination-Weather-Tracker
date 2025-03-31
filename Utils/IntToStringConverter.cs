using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace DestinationsApp.Utils 
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.TryParse(value?.ToString(), out var result) ? result : 0;
        }
    }
}
