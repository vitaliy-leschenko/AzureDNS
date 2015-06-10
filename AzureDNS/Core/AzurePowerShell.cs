using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using AzureDNS.ViewModels;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;

namespace AzureDNS.Core
{
    public class AzurePowerShell
    {
        private readonly Runspace runspace;
        private readonly ILoggerFacade logger;
        private readonly IUnityContainer container;

        public AzurePowerShell(Runspace runspace, ILoggerFacade logger, IUnityContainer container)
        {
            this.runspace = runspace;
            this.logger = logger;
            this.container = container;
        }


        public Task SelectAzureSubscriptionAsync(Guid subscriptionId)
        {
            return Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var ps = PowerShell.Create();
                        ps.Runspace = runspace;
                        ps.AddCommand("Select-AzureSubscription")
                            .AddParameter("SubscriptionId", subscriptionId)
                            .AddParameter("Current", true)
                            .Invoke();
                    }
                });
        }

        public async Task<IList<SubscriptionViewModel>> GetAzureSubscriptionAsync()
        {
            return await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var ps = PowerShell.Create();
                        ps.Runspace = runspace;
                        var items = ps.AddCommand("Get-AzureSubscription").Invoke().ToList();

                        var result = new List<SubscriptionViewModel>();
                        foreach (var o in items)
                        {
                            dynamic item = o.BaseObject;

                            var subscription = new SubscriptionViewModel();
                            subscription.SubscriptionId = new Guid((string)item.SubscriptionId);
                            subscription.SubscriptionName = item.SubscriptionName;
                            subscription.IsCurrent = item.IsCurrent;

                            result.Add(subscription);
                        }

                        return result;
                    }
                });
        }

        public async Task<IList<DnsZoneViewModel>> GetAzureDnsZoneAsync()
        {
            return await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(new Command("Get-AzureResourceGroup"));
                        pipe.Commands.Add(new Command("Get-AzureDnsZone"));

                        var result = pipe.Invoke();
                        var list = new List<DnsZoneViewModel>();

                        foreach (var o in result)
                        {
                            dynamic zoneList = o.BaseObject;
                            foreach (var item in zoneList)
                            {
                                var zone = new DnsZoneViewModel();
                                zone.Name = item.Name;
                                zone.ResourceGroupName = item.ResourceGroupName;

                                list.Add(zone);
                            }
                        }

                        return list;
                    }
                });
        }

        public async Task<IList<DnsRecordViewModel>> GetAzureDnsRecordsAsync(DnsZoneViewModel zone)
        {
            return await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        try
                        {
                            var ps = PowerShell.Create();
                            ps.Runspace = runspace;
                            var items = ps.AddCommand("Get-AzureDnsRecordSet")
                                .AddParameter("ResourceGroupName", zone.ResourceGroupName)
                                .AddParameter("ZoneName", zone.Name)
                                .Invoke();

                            dynamic records = items[0].BaseObject;

                            var result = new List<DnsRecordViewModel>();
                            foreach (var item in records)
                            {
                                string recordType = item.RecordType.ToString();

                                var record = new DnsRecordViewModel();
                                record.Name = item.Name;
                                record.RecordType = (RecordType) Enum.Parse(typeof (RecordType), recordType);
                                var reader = container.Resolve<IDnsRecordReader>(recordType);

                                var list = new List<BaseDnsRecord>();
                                foreach (var r in item.Records)
                                {
                                    list.Add(reader.Read(r));
                                }
                                record.Records = list;

                                result.Add(record);
                            }
                            return result;
                        }
                        catch (Exception)
                        {
                            return new List<DnsRecordViewModel>();
                        }
                    }
                });
        }

        public async Task<bool> AddAzureAccount()
        {
            return await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var ps = PowerShell.Create();
                        ps.Runspace = runspace;
                        var result = ps.AddCommand("Add-AzureAccount").Invoke();
                        return result.Count != 0;
                    }
                });
        }

        public void InitializeAzureResourceManager()
        {
            var ps = PowerShell.Create();
            ps.Runspace = runspace;
            ps.AddCommand("Switch-AzureMode")
                .AddParameter("Name", "AzureResourceManager")
                .Invoke();
        }
    }
}
