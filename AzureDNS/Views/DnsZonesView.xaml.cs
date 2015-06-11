using System.Windows;
using System.Windows.Controls;
using AzureDNS.ViewModels;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsZonesView : UserControl, IDnsZonesView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (DnsZonesViewModel), typeof (DnsZonesView), new PropertyMetadata(default(DnsZonesViewModel)));

        public DnsZonesViewModel ViewModel
        {
            get { return (DnsZonesViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsZonesView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsZonesViewModel>(new ParameterOverride("view", this));
        }
    }
}
