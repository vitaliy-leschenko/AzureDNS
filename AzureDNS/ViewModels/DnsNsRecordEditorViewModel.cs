﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Core;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class DnsNsRecordEditorViewModel: BaseViewModel
    {
        private readonly IDnsNsRecordEditor view;
        private readonly IUnityContainer container;
        private bool editMode;
        private DnsZoneViewModel dnsZone;
        private DnsRecordViewModel dnsRecord;
        private string hostName = string.Empty;
        private string ns = string.Empty;
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
                NS = string.Join(Environment.NewLine, value.Records.OfType<NsDnsRecord>().Select(t => t.Nsdname));
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

        public string NS
        {
            get { return ns; }
            set
            {
                ns = value;
                OnPropertyChanged();
            }
        }

        public DnsNsRecordEditorViewModel(IDnsNsRecordEditor view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;
        }
    }
}