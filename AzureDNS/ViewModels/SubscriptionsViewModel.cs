using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class SubscriptionsViewModel : BaseViewModel
    {
        private readonly IUnityContainer container;
        private readonly AzurePowerShell shell;
        private Visibility listVisibility = Visibility.Visible;
        private Visibility loadingVisibility = Visibility.Collapsed;
        private readonly ObservableCollection<SubscriptionViewModel> subscriptions = new ObservableCollection<SubscriptionViewModel>();
        private SubscriptionViewModel current;
        private bool isEnabled = true;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public Visibility ListVisibility
        {
            get { return listVisibility; }
            set
            {
                listVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set
            {
                loadingVisibility = value;
                OnPropertyChanged();
            }
        }

        public SubscriptionViewModel Current
        {
            get { return current; }
            set
            {
                current = value;
                SelectCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SubscriptionViewModel> Subscriptions
        {
            get { return subscriptions; }
        }

        public SubscriptionsViewModel(IUnityContainer container)
        {
            this.container = container;
            shell = container.Resolve<AzurePowerShell>();

            SelectCommand = new DelegateCommand(OnSelectClick, () => Current != null);
            LoadData();
        }

        public DelegateCommand SelectCommand { get; set; }

        private async void LoadData()
        {
            try
            {
                LoadingVisibility = Visibility.Visible;
                ListVisibility = Visibility.Collapsed;

                subscriptions.Clear();
                foreach (var item in await shell.GetAzureSubscriptionAsync())
                {
                    subscriptions.Add(item);
                }
            }
            finally
            {
                ListVisibility = Visibility.Visible;
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async void OnSelectClick()
        {
            try
            {
                IsEnabled = false;
                LoadingVisibility = Visibility.Visible;

                await shell.SelectAzureSubscriptionAsync(Current.SubscriptionId);
            }
            finally 
            {
                IsEnabled = true;
                LoadingVisibility = Visibility.Collapsed;
            }

            var window = Application.Current.MainWindow;
            var view = container.Resolve<ResourcesView>();
            view.Owner = window;
            view.ShowDialog();
        }
    }
}
