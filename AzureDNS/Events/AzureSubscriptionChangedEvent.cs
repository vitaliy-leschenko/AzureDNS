using Microsoft.Practices.Prism.PubSubEvents;

namespace AzureDNS.Events
{
    public class AzureSubscriptionChangedEvent : PubSubEvent<string>
    {
    }
}