using System.Windows;
using AzureDNS.ViewModels;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsMxRecordEditorView : Window, IDnsMxRecordEditor
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DnsMxRecordEditorViewModel), typeof(DnsMxRecordEditorView), new PropertyMetadata(default(DnsMxRecordEditorViewModel)));

        public DnsMxRecordEditorViewModel ViewModel
        {
            get { return (DnsMxRecordEditorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsMxRecordEditorView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsMxRecordEditorViewModel>(new ParameterOverride("view", this));
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
