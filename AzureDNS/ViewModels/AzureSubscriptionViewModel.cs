using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Events;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class AzureSubscriptionViewModel: BaseViewModel
    {
        private readonly IAzureSubscriptionView view;
        private readonly IUnityContainer container;
        private readonly ILoggerFacade logger;
        private readonly ObservableCollection<SubscriptionViewModel> subscriptions = new ObservableCollection<SubscriptionViewModel>();
        private SubscriptionViewModel current;
        private bool isEnabled = true;
        private bool loading;
        private ICommand addAccountCommand;

        public AzureSubscriptionViewModel(IAzureSubscriptionView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;
            logger = container.Resolve<ILoggerFacade>();

            view.Loaded += OnLoaded;

            AddAccountCommand = new DelegateCommand(OnAddAccountClick);
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

        public ICommand AddAccountCommand
        {
            get { return addAccountCommand; }
            set
            {
                addAccountCommand = value;
                OnPropertyChanged();
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadSubscriptionsAsync();
        }

        private async Task LoadSubscriptionsAsync()
        {
            logger.Log("Getting AzureSubscription...", Category.Info, Priority.Low);

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

                Current = items.FirstOrDefault(t => t.IsCurrent);
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

        private async void LoadDnsZonesAsync()
        {
            if (Current == null) return;

            try
            {
                Loading = true;
                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();
                await ps.SelectAzureSubscriptionAsync(Current.SubscriptionId);

                var aggregator = container.Resolve<IEventAggregator>();
                aggregator.GetEvent<DnsZoneChangedEvent>().Publish(null);
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
