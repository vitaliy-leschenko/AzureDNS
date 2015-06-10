using System.Windows;
using System.Windows.Controls;
using AzureDNS.Controls;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public interface ILogView
    {
        event RoutedEventHandler Loaded;
        event RoutedEventHandler Unloaded;
        void FocusItem(LogMessage item);
    }

    public partial class LogView : UserControl, ILogView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (LogViewModel), typeof (LogView), new PropertyMetadata(default(LogViewModel)));

        public LogViewModel ViewModel
        {
            get { return (LogViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public LogView()
        {
            InitializeComponent();
        }

        public LogView(IUnityContainer container): this()
        {
            ViewModel = container.Resolve<LogViewModel>(new ParameterOverride("view", this));
        }

        public void FocusItem(LogMessage item)
        {
            list.SelectedItem = item;
            list.ScrollIntoView(item);
        }
    }
}
