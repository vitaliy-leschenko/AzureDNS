using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.PubSubEvents;

namespace AzureDNS.Controls
{
    public class LogMessage
    {
        public string Message { get; set; }
        public Category Category { get; set; }
        public Priority Priority { get; set; }
    }

    public class LogEvent : PubSubEvent<LogMessage>
    {
    }
}
