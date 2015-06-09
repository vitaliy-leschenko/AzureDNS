using System.Diagnostics;
using Microsoft.Practices.Prism.Logging;

namespace AzureDNS.Common
{
    class AppLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            Trace.WriteLine(message, category.ToString());
        }
    }
}