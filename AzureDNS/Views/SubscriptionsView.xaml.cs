using System.Windows;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class SubscriptionsView : Window
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (SubscriptionsViewModel), typeof (SubscriptionsView), new PropertyMetadata(default(SubscriptionsViewModel)));

        [Dependency]
        public SubscriptionsViewModel ViewModel
        {
            get { return (SubscriptionsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public SubscriptionsView()
        {
            InitializeComponent();
        }
    }
}
