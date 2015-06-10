using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class MainPageViewModel: BaseViewModel
    {
        private readonly IMainPageView view;
        private readonly IUnityContainer container;

        private readonly ObservableCollection<DnsZoneViewModel> zones = new ObservableCollection<DnsZoneViewModel>();
        private readonly ObservableCollection<DnsRecordViewModel> records = new ObservableCollection<DnsRecordViewModel>();

        private bool isEnabled = true;
        private bool loading = false;
        private string subscriptionName;
        private ICommand selectSubscriptionCommand;
        private readonly Window window;
        private DnsZoneViewModel currentZone;

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
            }
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

        public ObservableCollection<DnsZoneViewModel> Zones
        {
            get { return zones; }
        }

        public DnsZoneViewModel CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                OnPropertyChanged();
                LoadDnsRecordsAsync();
            }
        }

        public ObservableCollection<DnsRecordViewModel> Records
        {
            get { return records; }
        }

        public MainPageViewModel(IMainPageView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;

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

                var ps = container.Resolve<AzurePowerShell>();
                await ps.InitializeAzureResourceManager();

                var subscriptions = await ps.GetAzureSubscriptionAsync();

                var currentSubscription = subscriptions.FirstOrDefault(t => t.IsCurrent);
                if (currentSubscription != null)
                {
                    SubscriptionName = currentSubscription.SubscriptionName;
                    await LoadDnsZonesAsync();
                }
            }
            catch (Exception)
            {
                SubscriptionName = string.Empty;
                Zones.Clear();
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }

        private async Task LoadDnsZonesAsync()
        {
            var ps = container.Resolve<AzurePowerShell>();
            var items = await ps.GetAzureDnsZoneAsync();
            Zones.Clear();
            foreach (var item in items)
            {
                Zones.Add(item);
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

        private async void LoadDnsRecordsAsync()
        {
            if (CurrentZone == null)
            {
                Records.Clear();
                return;
            }

            try
            {
                IsEnabled = false;
                Loading = true;

                var ps = container.Resolve<AzurePowerShell>();
                var items = await ps.GetAzureDnsRecordsAsync(CurrentZone);

                Records.Clear();
                foreach (var item in items)
                {
                    Records.Add(item);
                }
            }
            catch (Exception)
            {
                Records.Clear();
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }
    }
}
