using System.Windows;
using System.Windows.Controls;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class AzureSubscriptionView : UserControl, IAzureSubscriptionView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (AzureSubscriptionViewModel), typeof (AzureSubscriptionView), new PropertyMetadata(default(AzureSubscriptionViewModel)));

        public AzureSubscriptionViewModel ViewModel
        {
            get { return (AzureSubscriptionViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public AzureSubscriptionView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<AzureSubscriptionViewModel>(new ParameterOverride("view", this));
        }
    }
}
