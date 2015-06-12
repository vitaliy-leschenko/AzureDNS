using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Events;
using AzureDNS.Views;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Prism.Commands;
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
        private DelegateCommand addZoneCommand;
        private DelegateCommand removeZoneCommand;
        private string currentSubscription;
        private DelegateCommand refreshZonesCommand;

        public DnsZonesViewModel(IDnsZonesView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;
            eventAggregator = container.Resolve<IEventAggregator>();
            logger = container.Resolve<ILoggerFacade>();

            view.Loaded += OnLoaded;
            view.Unloaded += OnUnloaded;

            AddZoneCommand = new DelegateCommand(OnAddZoneClick, () => !string.IsNullOrEmpty(currentSubscription));
            RemoveZoneCommand = new DelegateCommand(OnRemoveClick, () => CurrentZone != null);
            RefreshZonesCommand = new DelegateCommand(OnRefreshClick, () => !string.IsNullOrEmpty(currentSubscription) && !Loading);
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
                RefreshZonesCommand.RaiseCanExecuteChanged();
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
                if (currentZone != value)
                {
                    currentZone = value;
                    OnPropertyChanged();
                    LoadDnsRecordsAsync();
                    RemoveZoneCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand AddZoneCommand
        {
            get { return addZoneCommand; }
            set
            {
                addZoneCommand = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand RemoveZoneCommand
        {
            get { return removeZoneCommand; }
            set
            {
                removeZoneCommand = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand RefreshZonesCommand
        {
            get { return refreshZonesCommand; }
            set
            {
                refreshZonesCommand = value;
                OnPropertyChanged();
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

        private async void OnAzureSubscriptionChange(string subscription)
        {
            currentSubscription = subscription;
            AddZoneCommand.RaiseCanExecuteChanged();
            RefreshZonesCommand.RaiseCanExecuteChanged();
            await LoadDnsZonesAsync();
        }

        private void LoadDnsRecordsAsync()
        {
            var aggregator = container.Resolve<IEventAggregator>();
            aggregator.GetEvent<DnsZoneChangedEvent>().Publish(CurrentZone);
        }

        private async Task LoadDnsZonesAsync()
        {
            try
            {
                Loading = true;
                IsEnabled = false;

                CurrentZone = null;
                if (string.IsNullOrEmpty(currentSubscription)) return;

                var ps = container.Resolve<AzurePowerShell>();
                var items = await ps.GetAzureDnsZoneAsync();

                Zones.Clear();
                if (string.IsNullOrEmpty(currentSubscription)) return;
                foreach (var item in items)
                {
                    Zones.Add(item);
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message, Category.Exception, Priority.High);
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }
        private async void OnAddZoneClick()
        {
            var wizard = container.Resolve<AddDnsZoneView>();
            wizard.Owner = Application.Current.MainWindow;
            if (wizard.ShowDialog() ?? false)
            {
                await LoadDnsZonesAsync();
            }
        }

        private async void OnRemoveClick()
        {
            var message = "Do you want to remove zone '" + CurrentZone.Name + "'?";
            if (MessageBox.Show(message, "Remove DNS zone", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Loading = true;
                    IsEnabled = false;

                    var ps = container.Resolve<AzurePowerShell>();

                    var zoneName = CurrentZone.Name;
                    var resourceGroupName = CurrentZone.ResourceGroupName;

                    CurrentZone = null;
                    await ps.RemoveDnsZoneAsync(zoneName, resourceGroupName);

                    await LoadDnsZonesAsync();
                }
                catch (Exception ex)
                {
                    logger.Log(ex.Message, Category.Exception, Priority.High);
                }
                finally
                {
                    Loading = false;
                    IsEnabled = true;
                }
            }
        }

        private async void OnRefreshClick()
        {
            CurrentZone = null;
            await LoadDnsZonesAsync();
        }
    }
}
