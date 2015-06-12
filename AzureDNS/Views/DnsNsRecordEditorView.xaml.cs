using System.Windows;
using AzureDNS.ViewModels;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsNsRecordEditorView : Window, IDnsNsRecordEditor
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DnsNsRecordEditorViewModel), typeof(DnsNsRecordEditorView), new PropertyMetadata(default(DnsNsRecordEditorViewModel)));

        public DnsNsRecordEditorViewModel ViewModel
        {
            get { return (DnsNsRecordEditorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsNsRecordEditorView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsNsRecordEditorViewModel>(new ParameterOverride("view", this));
        }

        public bool EditMode
        {
            get { return ViewModel.EditMode; }
            set { ViewModel.EditMode = value; }
        }

        public DnsZoneViewModel DnsZone
        {
            get { return ViewModel.DnsZone; }
            set { ViewModel.DnsZone = value; }
        }

        public DnsRecordViewModel DnsRecord
        {
            get { return ViewModel.DnsRecord; }
            set { ViewModel.DnsRecord = value; }
        }

        public void Complete()
        {
            DialogResult = true;
            Hide();
        }
    }
}
