using Microsoft.Practices.Prism.Logging;

namespace AzureDNS.Events
{
    public class LogMessage
    {
        public string Message { get; set; }
        public Category Category { get; set; }
        public Priority Priority { get; set; }
    }
}