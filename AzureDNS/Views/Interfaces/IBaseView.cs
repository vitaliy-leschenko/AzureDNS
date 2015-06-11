using System.Windows;

namespace AzureDNS.Views.Interfaces
{
    public interface IBaseView
    {
        event RoutedEventHandler Loaded;
        event RoutedEventHandler Unloaded;
    }
}