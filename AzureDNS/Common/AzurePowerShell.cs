using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using AzureDNS.ViewModels;

namespace AzureDNS.Common
{
    public class AzurePowerShell
    {
        private readonly Runspace runspace;

        public AzurePowerShell(Runspace runspace)
        {
            this.runspace = runspace;
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
                        var items = ps.AddCommand("Get-AzureSubscription").Invoke<dynamic>().ToList();

                        var result = new List<SubscriptionViewModel>();
                        foreach (var item in items)
                        {
                            var subscription = new SubscriptionViewModel();
                            subscription.SubscriptionId = new Guid((string)item.SubscriptionId);
                            subscription.SubscriptionName = item.SubscriptionName;

                            result.Add(subscription);
                        }

                        return result;
                    }
                });
        }

        public async Task<IList<string>> GetAzureResourceGroupAsync()
        {
            return await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var ps = PowerShell.Create();
                        ps.Runspace = runspace;
                        var items = ps.AddCommand("Get-AzureResourceGroup").Invoke<dynamic>();
                        return items.Select(item => item.ResourceGroupName).Cast<string>().ToList();
                    }
                });
        }

        public async Task<IList<string>> GetAzureDnsZoneAsync(string resourceGroupName)
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
                            var items = ps.AddCommand("Get-AzureDnsZone")
                                .AddParameter("ResourceGroupName", resourceGroupName)
                                .Invoke();
                            return items.Select(item => ((dynamic)item).Name).Cast<string>().ToList();
                        }
                        catch (Exception)
                        {
                            return new List<string>();
                        }
                    }
                });
        }

        public async Task<IList<DnsRecordViewModel>> GetAzureDnsRecordsAsync(string resourceGroupName, string zoneName)
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
                                .AddParameter("ResourceGroupName", resourceGroupName)
                                .AddParameter("ZoneName", zoneName)
                                .Invoke();

                            dynamic records = items[0].BaseObject;

                            var result = new List<DnsRecordViewModel>();
                            foreach (var item in records)
                            {
                                var record = new DnsRecordViewModel();
                                record.Name = item.Name;
                                record.RecordType = item.RecordType.ToString();

                                var list = new List<object>();
                                foreach (var r in item.Records)
                                {
                                    list.Add(r);
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

        public bool AddAzureAccount()
        {
            var ps = PowerShell.Create();
            ps.Runspace = runspace;
            var result = ps.AddCommand("Add-AzureAccount").Invoke();
            return result.Count != 0;
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
