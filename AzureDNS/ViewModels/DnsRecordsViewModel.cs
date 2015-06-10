using System;
using System.Collections.ObjectModel;
using System.Windows;
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
    public class DnsRecordsViewModel: BaseViewModel
    {
        private readonly IUnityContainer container;
        private readonly IDnsRecordsView view;
        private readonly ObservableCollection<DnsRecordViewModel> records = new ObservableCollection<DnsRecordViewModel>();
        private DnsRecordViewModel currentRecord;
        private DelegateCommand editRecordCommand;
        private DelegateCommand<object> addRecordCommand;
        private readonly IEventAggregator eventAggregator;
        private readonly ILoggerFacade logger;
        private bool loading;
        private bool isEnabled;
        private DnsZoneViewModel currentZone;

        public ObservableCollection<DnsRecordViewModel> Records
        {
            get { return records; }
        }

        public DnsRecordViewModel CurrentRecord
        {
            get { return currentRecord; }
            set
            {
                currentRecord = value;
                OnPropertyChanged();
                EditRecordCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand EditRecordCommand
        {
            get { return editRecordCommand; }
            set
            {
                editRecordCommand = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand<object> AddRecordCommand
        {
            get { return addRecordCommand; }
            set
            {
                addRecordCommand = value;
                OnPropertyChanged();
            }
        }

        public DnsRecordsViewModel(IDnsRecordsView view, IUnityContainer container)
        {
            this.container = container;
            this.view = view;
            eventAggregator = container.Resolve<IEventAggregator>();
            logger = container.Resolve<ILoggerFacade>();

            view.Loaded += OnLoaded;
            view.Unloaded += OnUnloaded;

            AddRecordCommand = new DelegateCommand<object>(OnAddDnsRecordClick, t => currentZone != null);
            EditRecordCommand = new DelegateCommand(OnEditDnsRecordClick, () => currentZone != null && currentRecord != null && currentRecord.AllowEdit);
        }

        private void OnEditDnsRecordClick()
        {
        }

        private void OnAddDnsRecordClick(object obj)
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<DnsZoneChangedEvent>().Subscribe(OnDnsZoneChange, ThreadOption.UIThread);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<DnsZoneChangedEvent>().Unsubscribe(OnDnsZoneChange);
        }

        private async void OnDnsZoneChange(DnsZoneViewModel zone)
        {
            currentZone = zone;
            AddRecordCommand.RaiseCanExecuteChanged();
            EditRecordCommand.RaiseCanExecuteChanged();

            if (zone == null)
            {
                Records.Clear();
                return;
            }

            try
            {
                IsEnabled = false;
                Loading = true;

                logger.Log("Getting AzureDnsRecords...", Category.Info, Priority.Low);

                var ps = container.Resolve<AzurePowerShell>();
                var items = await ps.GetAzureDnsRecordsAsync(zone);

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
                logger.Log("Done", Category.Info, Priority.Low);

                Loading = false;
                IsEnabled = true;
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

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }
    }
}
