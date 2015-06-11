using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AzureDNS.Controls
{
    public class NegativeToVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = System.Convert.ToBoolean(value);
            return data ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
