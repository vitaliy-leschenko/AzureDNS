using System.Windows;
using System.Windows.Controls;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsRecordsView : UserControl, IDnsRecordsView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (DnsRecordsViewModel), typeof (DnsRecordsView), new PropertyMetadata(default(DnsRecordsViewModel)));

        public DnsRecordsViewModel ViewModel
        {
            get { return (DnsRecordsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsRecordsView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsRecordsViewModel>(new ParameterOverride("view", this));
        }
    }
}
