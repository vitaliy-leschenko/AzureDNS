using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class DnsMxRecordEditorViewModel: BaseViewModel
    {
        private readonly IDnsMxRecordEditor view;
        private readonly IUnityContainer container;
        private bool editMode;
        private DnsZoneViewModel dnsZone;
        private DnsRecordViewModel dnsRecord;
        private string hostName = string.Empty;
        private string mxList = string.Empty;
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
                MxList = string.Join(Environment.NewLine, value.Records.OfType<MxDnsRecord>().Select(t => t.ToString()));
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

        public string MxList
        {
            get { return mxList; }
            set
            {
                mxList = value;
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

        public DnsMxRecordEditorViewModel(IDnsMxRecordEditor view, IUnityContainer container)
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

                var lines = MxList.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToArray();

                var records = (
                    from line in lines
                    let p = line.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                    where p.Length == 2
                    let priority = Convert.ToInt32(p[0])
                    let host = p[1]
                    select new Dictionary<string, string>
                    {
                        {"Preference", priority.ToString()},
                        {"Exchange", host}
                    }
                ).ToList();

                await ps.AddDnsRecordAsync(dnsZone, name, "MX", options, records, EditMode);
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
