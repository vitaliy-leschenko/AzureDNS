using System.Windows;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsARecordEditorView : Window, IDnsARecordEditor
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (DnsARecordEditorViewModel), typeof (DnsARecordEditorView), new PropertyMetadata(default(DnsARecordEditorViewModel)));

        public DnsARecordEditorViewModel ViewModel
        {
            get { return (DnsARecordEditorViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsARecordEditorView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsARecordEditorViewModel>(new ParameterOverride("view", this));
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

        public void FocusHostName()
        {
            hostName.Focus();
        }

        public void Complete()
        {
            DialogResult = true;
            Hide();
        }
    }
}
