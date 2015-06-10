using System.Windows;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class SubscriptionsView : Window, ISubscriptionsView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (SubscriptionsViewModel), typeof (SubscriptionsView), new PropertyMetadata(default(SubscriptionsViewModel)));

        public SubscriptionsViewModel ViewModel
        {
            get { return (SubscriptionsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public SubscriptionsView()
        {
            InitializeComponent();
        }

        public SubscriptionsView(IUnityContainer container): this()
        {
            ViewModel = container.Resolve<SubscriptionsViewModel>(new ParameterOverride("view", this));
        }

        public void Complete()
        {
            DialogResult = true;
            Hide();
        }
    }
}
