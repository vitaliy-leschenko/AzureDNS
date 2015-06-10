using System;
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
    public class MainPageViewModel: BaseViewModel
    {
        private readonly IMainPageView view;
        private readonly IUnityContainer container;
        private readonly ILoggerFacade logger;

        private bool isEnabled = true;
        private bool loading = false;
        private string subscriptionName;
        private ICommand selectSubscriptionCommand;
        private readonly Window window;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
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

        public string SubscriptionName
        {
            get { return subscriptionName; }
            set
            {
                subscriptionName = value;
                OnPropertyChanged();
                LoadDnsZonesAsync();
            }
        }

        private void LoadDnsZonesAsync()
        {
            var aggregator = container.Resolve<IEventAggregator>();
            aggregator.GetEvent<AzureSubscriptionChangedEvent>().Publish(SubscriptionName);
        }

        public ICommand SelectSubscriptionCommand
        {
            get { return selectSubscriptionCommand; }
            set
            {
                selectSubscriptionCommand = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel(IMainPageView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;

            logger = container.Resolve<ILoggerFacade>();

            window = (Window) view;
            window.Loaded += OnLoaded;

            SelectSubscriptionCommand = new DelegateCommand(OnSelectSubscriptionClick);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadSubscriptionAsync();
        }

        private async Task LoadSubscriptionAsync()
        {
            try
            {
                IsEnabled = false;
                Loading = true;

                logger.Log("Getting AzureSubscriptions...", Category.Info, Priority.Low);

                var ps = container.Resolve<AzurePowerShell>();
                await ps.InitializeAzureResourceManager();

                var subscriptions = await ps.GetAzureSubscriptionAsync();

                var currentSubscription = subscriptions.FirstOrDefault(t => t.IsCurrent);
                if (currentSubscription != null)
                {
                    SubscriptionName = currentSubscription.SubscriptionName;
                }
            }
            catch (Exception)
            {
                SubscriptionName = string.Empty;
            }
            finally
            {
                logger.Log("Done", Category.Info, Priority.Low);

                Loading = false;
                IsEnabled = true;
            }
        }

        private async void OnSelectSubscriptionClick()
        {
            var dialog = container.Resolve<SubscriptionsView>();
            dialog.Owner = window;

            if (dialog.ShowDialog() ?? false)
            {
                await LoadSubscriptionAsync();
            }
        }
    }
}
