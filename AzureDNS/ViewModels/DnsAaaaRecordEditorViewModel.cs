using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class DnsAaaaRecordEditorViewModel: BaseViewModel
    {
        private readonly IDnsAaaaRecordEditor view;
        private readonly IUnityContainer container;
        private bool editMode;
        private DnsZoneViewModel dnsZone;
        private DnsRecordViewModel dnsRecord;
        private string hostName = string.Empty;
        private string ip = string.Empty;
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
                IP = string.Join(";", value.Records.OfType<AaaaDnsRecord>().Select(t => t.Ipv6Address));
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

        public DnsAaaaRecordEditorViewModel(IDnsAaaaRecordEditor view, IUnityContainer container)
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
                var name = HostName.Trim();

                IsEnabled = false;

                var ps = container.Resolve<AzurePowerShell>();

                var options = new Dictionary<string, object> {{"Ttl", 300}};

                var addresses = IP.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim())
                    .ToArray();
                var records = addresses.Select(t => new Dictionary<string, string> { { "Ipv6Address", t } }).ToList();

                await ps.AddDnsRecordAsync(dnsZone, name, "AAAA", options, records, EditMode);
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
