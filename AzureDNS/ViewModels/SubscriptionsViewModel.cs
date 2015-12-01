using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Events;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class SubscriptionsViewModel: BaseViewModel
    {
        private readonly ISubscriptionsView view;
        private readonly IUnityContainer container;
        private readonly ObservableCollection<SubscriptionViewModel> subscriptions = new ObservableCollection<SubscriptionViewModel>();
        private SubscriptionViewModel current;
        private bool isEnabled = true;
        private bool loading;
        private DelegateCommand addAccountCommand;
        private DelegateCommand refreshCommand;

        public SubscriptionsViewModel(ISubscriptionsView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;

            view.Loaded += OnLoaded;

            AddAccountCommand = new DelegateCommand(OnAddAccountClick, () => !Loading);
            RefreshCommand = new DelegateCommand(OnRefreshClick, () => !Loading);
        }

        public ObservableCollection<SubscriptionViewModel> Subscriptions
        {
            get { return subscriptions; }
        }

        public bool Loading
        {
            get { return loading; }
            set
            {
                loading = value;
                OnPropertyChanged();
                AddAccountCommand.RaiseCanExecuteChanged();
                RefreshCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public SubscriptionViewModel Current
        {
            get { return current; }
            set
            {
                current = value;
                OnPropertyChanged();
                LoadDnsZonesAsync();
            }
        }

        public DelegateCommand AddAccountCommand
        {
            get { return addAccountCommand; }
            set
            {
                addAccountCommand = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand RefreshCommand
        {
            get { return refreshCommand; }
            set
            {
                refreshCommand = value;
                OnPropertyChanged();
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadSubscriptionsAsync();
        }

        private async Task LoadSubscriptionsAsync()
        {
            try
            {
                Loading = true;
                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();

                var items = await ps.GetAzureSubscriptionAsync();
                subscriptions.Clear();
                foreach (var item in items)
                {
                    subscriptions.Add(item);
                }

                Current = items.FirstOrDefault();
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }

        private async void OnAddAccountClick()
        {
            try
            {
                Loading = true;
                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();
                var result = await ps.AddAzureAccount();
                if (result)
                {
                    await LoadSubscriptionsAsync();
                }
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }

        private async void OnRefreshClick()
        {
            Current = null;
            await LoadSubscriptionsAsync();
        }

        private async void LoadDnsZonesAsync()
        {
            try
            {
                Loading = true;
                IsEnabled = false;

                var aggregator = container.Resolve<IEventAggregator>();
                aggregator.GetEvent<DnsZoneChangedEvent>().Publish(null);

                if (Current == null)
                {
                    aggregator.GetEvent<AzureSubscriptionChangedEvent>().Publish(null);
                    return;
                }

                var ps = container.Resolve<AzurePowerShell>();
                await ps.SelectAzureSubscriptionAsync(Current.SubscriptionId);

                if (Current == null)
                {
                    aggregator.GetEvent<AzureSubscriptionChangedEvent>().Publish(null);
                    return;
                }

                aggregator.GetEvent<AzureSubscriptionChangedEvent>().Publish(Current.SubscriptionName);
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }
    }
}
