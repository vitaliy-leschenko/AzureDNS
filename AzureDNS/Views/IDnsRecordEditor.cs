using System.Windows;
using AzureDNS.ViewModels;

namespace AzureDNS.Views
{
    public interface IDnsRecordEditor: IBaseView
    {
        bool? ShowDialog();
        bool EditMode { get; set; }
        DnsZoneViewModel DnsZone { get; set; }
        DnsRecordViewModel DnsRecord { get; set; }
        Window Owner { get; set; }
    }
}