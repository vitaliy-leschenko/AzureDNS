using System.Windows;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class DnsTxtRecordEditorView : Window, IDnsTxtRecordEditor
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DnsTxtRecordEditorViewModel), typeof(DnsTxtRecordEditorView), new PropertyMetadata(default(DnsTxtRecordEditorViewModel)));

        public DnsTxtRecordEditorViewModel ViewModel
        {
            get { return (DnsTxtRecordEditorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public DnsTxtRecordEditorView(IUnityContainer container)
        {
            InitializeComponent();
            ViewModel = container.Resolve<DnsTxtRecordEditorViewModel>(new ParameterOverride("view", this));
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
