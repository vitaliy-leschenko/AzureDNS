using System;
using System.Collections.ObjectModel;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class AddDnsZoneViewModel: BaseViewModel
    {
        private readonly IAddDnsZoneView view;
        private readonly IUnityContainer container;
        private bool isEnabled = true;
        private string zoneName;
        private readonly ObservableCollection<string> groups = new ObservableCollection<string>();
        private bool loading;
        private DelegateCommand addCommand;
        private string resourceGroupName;

        public AddDnsZoneViewModel(IAddDnsZoneView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;

            AddCommand = new DelegateCommand(OnAddClick, () => !string.IsNullOrWhiteSpace(ZoneName) && !string.IsNullOrWhiteSpace(ResourceGroupName));

            view.Loaded += OnViewLoaded;
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

        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> Groups
        {
            get { return groups; }
        }

        public DelegateCommand AddCommand
        {
            get { return addCommand; }
            set
            {
                addCommand = value;
                OnPropertyChanged();
            }
        }

        public string ResourceGroupName
        {
            get { return resourceGroupName; }
            set
            {
                resourceGroupName = value;
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        private async void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Loading = true;
                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();
                var items = await ps.GetAzureResourceGroupsAsync();
                Groups.Clear();
                foreach (var item in items)
                {
                    Groups.Add(item);
                }
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }

        private async void OnAddClick()
        {
            try
            {
                Loading = true;
                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();
                await ps.AddDnsZoneAsync(ZoneName, ResourceGroupName);
                view.Complete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Loading = false;
                IsEnabled = true;
            }
        }
    }
}
