using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AzureDNS.Common;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class ResourcesViewModel: BaseViewModel
    {
        private readonly IUnityContainer container;
        private readonly AzurePowerShell shell;

        public ResourcesViewModel(IUnityContainer container, AzurePowerShell shell)
        {
            this.container = container;
            this.shell = shell;
            LoadResourceGroups();
        }

        #region Resource Groups

        private readonly ObservableCollection<string> resourceGroups = new ObservableCollection<string>();
        private string currentResourceGroupName;
        private Visibility resourceGroupsLoadingVisibility;
        private Visibility resourceGroupsListVisibility;
        private bool isGroupsEnabled = true;

        public bool IsGroupsEnabled
        {
            get { return isGroupsEnabled; }
            set
            {
                isGroupsEnabled = value;
                OnPropertyChanged();
            }
        }

        public string CurrentResourceGroupName
        {
            get { return currentResourceGroupName; }
            set
            {
                currentResourceGroupName = value;
                OnPropertyChanged();
                LoadAzureDnsZones();
            }
        }

        public ObservableCollection<string> ResourceGroups
        {
            get { return resourceGroups; }
        }

        public Visibility ResourceGroupsLoadingVisibility
        {
            get { return resourceGroupsLoadingVisibility; }
            set
            {
                resourceGroupsLoadingVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ResourceGroupsListVisibility
        {
            get { return resourceGroupsListVisibility; }
            set
            {
                resourceGroupsListVisibility = value;
                OnPropertyChanged();
            }
        }

        private async void LoadResourceGroups()
        {
            try
            {
                ResourceGroupsLoadingVisibility = Visibility.Visible;
                ResourceGroupsListVisibility = Visibility.Collapsed;

                ResourceGroups.Clear();
                foreach (var name in await shell.GetAzureResourceGroupAsync())
                {
                    ResourceGroups.Add(name);
                }
            }
            finally
            {
                CurrentResourceGroupName = null;
                ResourceGroupsLoadingVisibility = Visibility.Collapsed;
                ResourceGroupsListVisibility = Visibility.Visible;
            }
        }

        #endregion

        #region DNS Zones

        private readonly ObservableCollection<string> zones = new ObservableCollection<string>();
        private string currentZoneName;
        private Visibility zonesListVisibility = Visibility.Visible;
        private Visibility zonesLoadingVisibility = Visibility.Collapsed;
        private bool isZoneEnabled;

        public bool IsZoneEnabled
        {
            get { return isZoneEnabled; }
            set
            {
                isZoneEnabled = value;
                OnPropertyChanged();
            }
        }

        public string CurrentZoneName
        {
            get { return currentZoneName; }
            set
            {
                currentZoneName = value;
                OnPropertyChanged();
                LoadAzureDnsRecordSets();
            }
        }

        public Visibility ZonesListVisibility
        {
            get { return zonesListVisibility; }
            set
            {
                zonesListVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ZonesLoadingVisibility
        {
            get { return zonesLoadingVisibility; }
            set
            {
                zonesLoadingVisibility = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Zones
        {
            get { return zones; }
        }

        private async void LoadAzureDnsZones()
        {
            if (string.IsNullOrEmpty(CurrentResourceGroupName))
            {
                Zones.Clear();
                ZonesLoadingVisibility = Visibility.Collapsed;
                ZonesListVisibility = Visibility.Visible;
                CurrentZoneName = null;
                return;
            }
            try
            {
                IsGroupsEnabled = false;

                ZonesLoadingVisibility = Visibility.Visible;
                ZonesListVisibility = Visibility.Collapsed;

                var items = await shell.GetAzureDnsZoneAsync(CurrentResourceGroupName);
                Zones.Clear();
                foreach (var name in items)
                {
                    Zones.Add(name);
                }
            }
            finally
            {
                IsGroupsEnabled = true;

                ZonesLoadingVisibility = Visibility.Collapsed;
                ZonesListVisibility = Visibility.Visible;
                CurrentZoneName = null;
            }
        }

        #endregion

        #region DNS Records

        private readonly ObservableCollection<DnsRecordViewModel> records = new ObservableCollection<DnsRecordViewModel>();
        private DnsRecordViewModel currentRecord;
        private Visibility recordsListVisibility = Visibility.Visible;
        private Visibility recordsLoadingVisibility = Visibility.Collapsed;

        public DnsRecordViewModel CurrentRecord
        {
            get { return currentRecord; }
            set
            {
                currentRecord = value;
                OnPropertyChanged();
            }
        }

        public Visibility RecordsListVisibility
        {
            get { return recordsListVisibility; }
            set
            {
                recordsListVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility RecordsLoadingVisibility
        {
            get { return recordsLoadingVisibility; }
            set
            {
                recordsLoadingVisibility = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DnsRecordViewModel> Records
        {
            get { return records; }
        }

        private async void LoadAzureDnsRecordSets()
        {
            if (string.IsNullOrEmpty(CurrentResourceGroupName))
            {
                Records.Clear();
                RecordsLoadingVisibility = Visibility.Collapsed;
                RecordsListVisibility = Visibility.Visible;
                CurrentRecord = null;
                return;
            }
            try
            {
                IsZoneEnabled = false;
                IsGroupsEnabled = false;

                RecordsLoadingVisibility = Visibility.Visible;
                RecordsListVisibility = Visibility.Collapsed;

                var items = await shell.GetAzureDnsRecordsAsync(CurrentResourceGroupName, CurrentZoneName);
                Records.Clear();
                foreach (var item in items)
                {
                    Records.Add(item);
                }
            }
            finally
            {
                IsZoneEnabled = true;
                IsGroupsEnabled = true;

                RecordsLoadingVisibility = Visibility.Collapsed;
                RecordsListVisibility = Visibility.Visible;
                CurrentRecord = null;
            }
        }

        #endregion
    }
}
