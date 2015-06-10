using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Events;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class DnsZonesViewModel: BaseViewModel
    {
        private readonly IDnsZonesView view;
        private readonly IUnityContainer container;
        private readonly ILoggerFacade logger;
        private readonly IEventAggregator eventAggregator;
        private readonly ObservableCollection<DnsZoneViewModel> zones = new ObservableCollection<DnsZoneViewModel>();
        private DnsZoneViewModel currentZone;

        private bool isEnabled = true;
        private bool loading = false;

        public DnsZonesViewModel(IDnsZonesView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;
            eventAggregator = container.Resolve<IEventAggregator>();
            logger = container.Resolve<ILoggerFacade>();

            view.Loaded += OnLoaded;
            view.Unloaded += OnUnloaded;
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

        public bool Loading
        {
            get { return loading; }
            set
            {
                loading = value;
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<AzureSubscriptionChangedEvent>().Subscribe(OnAzureSubscriptionChange, ThreadOption.UIThread);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<AzureSubscriptionChangedEvent>().Unsubscribe(OnAzureSubscriptionChange);
        }

        private async void OnAzureSubscriptionChange(string obj)
        {
            await LoadDnsZonesAsync();
        }

        private void LoadDnsRecordsAsync()
        {
            var aggregator = container.Resolve<IEventAggregator>();
            aggregator.GetEvent<DnsZoneChangedEvent>().Publish(CurrentZone);
        }

        private async Task LoadDnsZonesAsync()
        {
            logger.Log("Getting AzureDnsZones...", Category.Info, Priority.Low);

            var ps = container.Resolve<AzurePowerShell>();
            var items = await ps.GetAzureDnsZoneAsync();
            Zones.Clear();
            foreach (var item in items)
            {
                Zones.Add(item);
            }
        }
    }
}
