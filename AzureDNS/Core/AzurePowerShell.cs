using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
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
                            .AddParameter("Current", true);

                        LogCommand(ps.Commands.Commands);
                        ps.Invoke();
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
                        ps.AddCommand("Get-AzureSubscription");

                        LogCommand(ps.Commands.Commands);
                        var items = ps.Invoke();

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

        public async Task<IList<string>> GetAzureResourceGroupsAsync()
        {
            return await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var pipe = runspace.CreatePipeline();
                        pipe.Commands.Add(new Command("Get-AzureResourceGroup"));

                        LogCommand(pipe.Commands);
                        var result = pipe.Invoke();
                        var list = new List<string>();

                        foreach (var o in result)
                        {
                            dynamic item = o.BaseObject;
                            list.Add(item.ResourceGroupName);
                        }

                        return list;
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

                        LogCommand(pipe.Commands);
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
                            ps.AddCommand("Get-AzureDnsRecordSet")
                                .AddParameter("ResourceGroupName", zone.ResourceGroupName)
                                .AddParameter("ZoneName", zone.Name);

                            LogCommand(ps.Commands.Commands);
                            var items = ps.Invoke();

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
                        ps.AddCommand("Add-AzureAccount");

                        LogCommand(ps.Commands.Commands);

                        var result = ps.Invoke();
                        return result.Count != 0;
                    }
                });
        }

        public void InitializeAzureResourceManager()
        {
            lock (runspace)
            {
                var ps = PowerShell.Create();
                ps.Runspace = runspace;
                ps.AddCommand("Switch-AzureMode").AddParameter("Name", "AzureResourceManager");

                LogCommand(ps.Commands.Commands);
                ps.Invoke();
            }
        }

        public async Task RemoveRecordSetAsync(DnsZoneViewModel dnsZone, DnsRecordViewModel record)
        {
            await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var pipe = runspace.CreatePipeline();

                        var rs = new Command("Remove-AzureDnsRecordSet");
                        rs.Parameters.Add("Name", record.Name);
                        rs.Parameters.Add("RecordType", record.RecordType.ToString());
                        rs.Parameters.Add("ZoneName", dnsZone.Name);
                        rs.Parameters.Add("ResourceGroupName", dnsZone.ResourceGroupName);
                        rs.Parameters.Add("Force");

                        pipe.Commands.Add(rs);

                        LogCommand(pipe.Commands);
                        pipe.Invoke();
                    }
                });
        }

        public async Task AddDnsRecordAsync(DnsZoneViewModel dnsZone, string hostName, string type, 
            Dictionary<string, object> options,
            List<Dictionary<string, string>> records, 
            bool overwrite = false)
        {
            await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var pipe = runspace.CreatePipeline();

                        var rs = new Command("New-AzureDnsRecordSet");
                        rs.Parameters.Add("Name", hostName);
                        rs.Parameters.Add("ZoneName", dnsZone.Name);
                        rs.Parameters.Add("ResourceGroupName", dnsZone.ResourceGroupName);
                        rs.Parameters.Add("RecordType", type);
                        foreach (var item in options)
                        {
                            rs.Parameters.Add(item.Key, item.Value);
                        }
                        if (overwrite)
                        {
                            rs.Parameters.Add("Overwrite");
                            rs.Parameters.Add("Force");
                        }

                        pipe.Commands.Add(rs);

                        foreach (var record in records)
                        {
                            var ip = new Command("Add-AzureDnsRecordConfig");

                            foreach (var item in record)
                            {
                                ip.Parameters.Add(item.Key, item.Value);
                            }

                            pipe.Commands.Add(ip);
                        }
                        pipe.Commands.Add(new Command("Set-AzureDnsRecordSet"));

                        LogCommand(pipe.Commands);
                        pipe.Invoke();

                        if (pipe.HadErrors)
                        {
                            dynamic error = pipe.Error.Read();
                            Exception ex = error.Exception;
                            throw new Exception("Can't create " + type + " record", ex);
                        }
                    }
                });
        }

        public async Task AddDnsZoneAsync(string zoneName, string resourceGroupName)
        {
            await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var ps = PowerShell.Create();
                        ps.Runspace = runspace;
                        ps.AddCommand("New-AzureDnsZone")
                            .AddParameter("Name", zoneName)
                            .AddParameter("ResourceGroupName", resourceGroupName);

                        LogCommand(ps.Commands.Commands);
                        ps.Invoke();

                        if (ps.HadErrors)
                        {
                            throw new Exception("Can't create DNS zone.");
                        }
                    }
                });
        }

        private void LogCommand(CommandCollection commands)
        {
            var output = new StringBuilder();

            foreach (var command in commands)
            {
                if (output.Length > 0) output.Append(" | ");

                output.Append(command.CommandText);
                foreach (var parameter in command.Parameters)
                {
                    var value = parameter.Value;
                    if (value is bool)
                    {
                        value = "$" + value;
                    }
                    else if (value is string)
                    {
                        var s = (string) value;
                        value = "\"" + s.Replace("\"", "\"\"") + "\"";
                    }
                    output.Append(" -" + parameter.Name + " " + value);
                }
            }
            
            logger.Log(output.ToString(), Category.Info, Priority.None);
        }

        public async Task RemoveDnsZoneAsync(string zoneName, string resourceGroupName)
        {
            await Task.Run(
                delegate
                {
                    lock (runspace)
                    {
                        var ps = PowerShell.Create();
                        ps.Runspace = runspace;
                        ps.AddCommand("Remove-AzureDnsZone")
                            .AddParameter("Name", zoneName)
                            .AddParameter("ResourceGroupName", resourceGroupName)
                            .AddParameter("Force");

                        LogCommand(ps.Commands.Commands);
                        ps.Invoke();

                        if (ps.HadErrors)
                        {
                            throw new Exception("Can't remove DNS zone.");
                        }
                    }
                });
        }
    }
}
