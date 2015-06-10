using Microsoft.Practices.Prism.PubSubEvents;

namespace AzureDNS.Events
{
    public class LogEvent : PubSubEvent<LogMessage>
    {
    }
}
