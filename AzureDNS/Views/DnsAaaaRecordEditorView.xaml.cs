using System.Windows;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsAaaaRecordEditorView : Window, IDnsAaaaRecordEditor
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DnsAaaaRecordEditorViewModel), typeof(DnsAaaaRecordEditorView), new PropertyMetadata(default(DnsAaaaRecordEditorViewModel)));

        public DnsAaaaRecordEditorViewModel ViewModel
        {
            get { return (DnsAaaaRecordEditorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsAaaaRecordEditorView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsAaaaRecordEditorViewModel>(new ParameterOverride("view", this));
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
