using System.Windows;
using System.Windows.Controls;
using AzureDNS.ViewModels;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class SubscriptionsView : UserControl, ISubscriptionsView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (SubscriptionsViewModel), typeof (SubscriptionsView), new PropertyMetadata(default(SubscriptionsViewModel)));

        public SubscriptionsViewModel ViewModel
        {
            get { return (SubscriptionsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public SubscriptionsView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<SubscriptionsViewModel>(new ParameterOverride("view", this));
        }
    }
}
