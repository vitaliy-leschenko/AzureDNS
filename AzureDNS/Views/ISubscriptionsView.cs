using System.Windows;

namespace AzureDNS.Views
{
    public interface ISubscriptionsView
    {
        event RoutedEventHandler Loaded;

        void Complete();
    }
}