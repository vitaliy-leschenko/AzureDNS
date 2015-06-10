using System.Windows;
using AzureDNS.ViewModels;
using Microsoft.Practices.Unity;

namespace AzureDNS.Views
{
    public interface IMainPageView
    {
    }

    public partial class MainPageView : Window, IMainPageView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (MainPageViewModel), typeof (MainPageView), new PropertyMetadata(default(MainPageViewModel)));

        public MainPageViewModel ViewModel
        {
            get { return (MainPageViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public MainPageView()
        {
            InitializeComponent();
        }

        public MainPageView(IUnityContainer container): this()
        {
            ViewModel = container.Resolve<MainPageViewModel>(new ParameterOverride("view", this));
        }
    }
}
