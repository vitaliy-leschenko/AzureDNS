using System.Windows;
using System.Windows.Controls;
using AzureDNS.Events;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class LogsView : UserControl, ILogView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (LogsViewModel), typeof (LogsView), new PropertyMetadata(default(LogsViewModel)));

        public LogsViewModel ViewModel
        {
            get { return (LogsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public LogsView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<LogsViewModel>(new ParameterOverride("view", this));
        }

        public void ScrollDown()
        {
            logView.ScrollToEnd();
        }
    }
}
