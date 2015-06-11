using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        private DelegateCommand<DnsRecordViewModel> editRecordCommand;
        private DelegateCommand<string> addRecordCommand;
        private readonly IEventAggregator eventAggregator;
        private readonly ILoggerFacade logger;
        private bool loading = false;
        private bool isEnabled = true;
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

        public DelegateCommand<DnsRecordViewModel> EditRecordCommand
        {
            get { return editRecordCommand; }
            set
            {
                editRecordCommand = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand<string> AddRecordCommand
        {
            get { return addRecordCommand; }
            set
            {
                addRecordCommand = value;
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

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
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

            AddRecordCommand = new DelegateCommand<string>(OnAddDnsRecordClick, t => currentZone != null);
            EditRecordCommand = new DelegateCommand<DnsRecordViewModel>(OnEditDnsRecordClick, t => currentZone != null && t != null && t.AllowEdit);
        }

        private async void OnEditDnsRecordClick(DnsRecordViewModel record)
        {
            IDnsRecordEditor editor;
            try
            {
                editor = container.Resolve<IDnsRecordEditor>(record.RecordType.ToString());
            }
            catch (Exception)
            {
                logger.Log("Can't create editor for "+record.RecordType+" record.", Category.Exception, Priority.Medium);
                return;
            }

            editor.EditMode = true;
            editor.DnsZone = currentZone;
            editor.DnsRecord = record;
            editor.Owner = Application.Current.MainWindow;

            if (editor.ShowDialog() ?? false)
            {
                await LoadDnsRecordsAsync();
            }
        }

        private async void OnAddDnsRecordClick(string type)
        {
            IDnsRecordEditor editor;
            try
            {
                editor = container.Resolve<IDnsRecordEditor>(type);
            }
            catch (Exception)
            {
                logger.Log("Can't create editor for " + type + " record.", Category.Exception, Priority.Medium);
                return;
            }

            editor.EditMode = false;
            editor.DnsZone = currentZone;
            editor.Owner = Application.Current.MainWindow;

            if (editor.ShowDialog() ?? false)
            {
                await LoadDnsRecordsAsync();
            }
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

            await LoadDnsRecordsAsync();
        }

        private async Task LoadDnsRecordsAsync()
        {
            try
            {
                IsEnabled = false;
                Loading = true;

                var ps = container.Resolve<AzurePowerShell>();
                var items = await ps.GetAzureDnsRecordsAsync(currentZone);

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
