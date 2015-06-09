using System.Windows;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public partial class ResourcesView : Window
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (ResourcesViewModel), typeof (ResourcesView), new PropertyMetadata(default(ResourcesViewModel)));

        [Dependency]
        public ResourcesViewModel ViewModel
        {
            get { return (ResourcesViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public ResourcesView()
        {
            InitializeComponent();
        }
    }
}
