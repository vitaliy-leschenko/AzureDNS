using AzureDNS.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;

namespace AzureDNS.Events
{
    public class DnsZoneChangedEvent : PubSubEvent<DnsZoneViewModel>
    {
    }
}