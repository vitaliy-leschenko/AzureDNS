using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AzureDNS.Controls
{
    public class BoolToVisibilityConverter: FrameworkElement, IValueConverter
    {
        public static readonly DependencyProperty HiddenVisibilityProperty = DependencyProperty.Register(
            "HiddenVisibility", typeof (Visibility), typeof (BoolToVisibilityConverter), new PropertyMetadata(default(Visibility)));

        public Visibility HiddenVisibility
        {
            get { return (Visibility) GetValue(HiddenVisibilityProperty); }
            set { SetValue(HiddenVisibilityProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = System.Convert.ToBoolean(value);
            return data ? Visibility.Visible : HiddenVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
