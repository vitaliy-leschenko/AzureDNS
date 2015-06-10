using System.Windows;
using System.Windows.Controls.Primitives;

namespace AzureDNS.Controls
{
    public class ItemButton : ButtonBase
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ItemButton), new PropertyMetadata(false));

        public ItemButton()
        {
            VerticalContentAlignment = VerticalAlignment.Center;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            Click += OnButtonClick;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
        }
    }
}
