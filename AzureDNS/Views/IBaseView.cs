using System.Windows;

namespace AzureDNS.Views
{
    public interface IBaseView
    {
        event RoutedEventHandler Loaded;
        event RoutedEventHandler Unloaded;
    }
}