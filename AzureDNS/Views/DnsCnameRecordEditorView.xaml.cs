using System.Windows;
using AzureDNS.ViewModels;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsCnameRecordEditorView : Window, IDnsCnameRecordEditor
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DnsCnameRecordEditorViewModel), typeof(DnsCnameRecordEditorView), new PropertyMetadata(default(DnsCnameRecordEditorViewModel)));

        public DnsCnameRecordEditorViewModel ViewModel
        {
            get { return (DnsCnameRecordEditorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsCnameRecordEditorView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsCnameRecordEditorViewModel>(new ParameterOverride("view", this));
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

        public void FocusCname()
        {
            cname.Focus();
        }

        public void Complete()
        {
            DialogResult = true;
            Hide();
        }
    }
}
