using System;
using System.Linq;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class DnsARecordEditorViewModel: BaseViewModel
    {
        private readonly IDnsARecordEditor view;
        private readonly IUnityContainer container;
        private bool editMode;
        private DnsZoneViewModel dnsZone;
        private DnsRecordViewModel dnsRecord;
        private string hostName;
        private string ip;
        private DelegateCommand saveCommand;
        private DelegateCommand deleteCommand;
        private bool isEnabled = true;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool EditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public DnsZoneViewModel DnsZone
        {
            get { return dnsZone; }
            set
            {
                dnsZone = value;
                OnPropertyChanged();
                OnPropertyChanged("FQDN");
            }
        }

        public DnsRecordViewModel DnsRecord
        {
            get { return dnsRecord; }
            set
            {
                dnsRecord = value;
                OnPropertyChanged();
                HostName = value.Name;
                IP = string.Join(";", value.Records.OfType<ADnsRecord>().Select(t => t.Ipv4Address));
            }
        }

        public string HostName
        {
            get { return hostName; }
            set
            {
                hostName = value;
                OnPropertyChanged();
                OnPropertyChanged("FQDN");
            }
        }

        public string FQDN
        {
            get
            {
                if (DnsZone == null) return string.Empty;
                var zoneName = DnsZone.Name;

                if (!string.IsNullOrWhiteSpace(HostName))
                {
                    var name = HostName.Trim();
                    if (name != "@")
                    {
                        return name + "." + zoneName;
                    }
                }
                return zoneName;
            }
        }

        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand SaveCommand
        {
            get { return saveCommand; }
            set
            {
                saveCommand = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand DeleteCommand
        {
            get { return deleteCommand; }
            set
            {
                deleteCommand = value;
                OnPropertyChanged();
            }
        }

        public DnsARecordEditorViewModel(IDnsARecordEditor view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;

            SaveCommand = new DelegateCommand(OnSaveClick);
            DeleteCommand = new DelegateCommand(OnDeleteClick, () => EditMode);
        }

        private async void OnSaveClick()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(HostName))
                {
                    view.FocusHostName();
                    return;
                }

                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();
                var addresses = IP.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim())
                    .ToArray();

                await ps.AddARecordAsync(dnsZone, HostName.Trim(), addresses, EditMode);
                view.Complete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private async void OnDeleteClick()
        {
            try
            {
                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();
                await ps.RemoveRecordSetAsync(dnsZone, dnsRecord);
                view.Complete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}
