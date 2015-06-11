using System.Windows;
using AzureDNS.ViewModels;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public interface IAddDnsZoneView : IBaseView
    {
        void Complete();
    }

    public partial class AddDnsZoneView : Window, IAddDnsZoneView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (AddDnsZoneViewModel), typeof (AddDnsZoneView), new PropertyMetadata(default(AddDnsZoneViewModel)));

        public AddDnsZoneViewModel ViewModel
        {
            get { return (AddDnsZoneViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public AddDnsZoneView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<AddDnsZoneViewModel>(new ParameterOverride("view", this));
        }

        public void Complete()
        {
            DialogResult = true;
            Hide();
        }
    }
}
