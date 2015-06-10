using System;
using System.Diagnostics;
using AzureDNS.Controls;
using AzureDNS.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace AzureDNS.Common
{
    class AppLogger : ILoggerFacade
    {
        private readonly IUnityContainer container;

        public AppLogger(IUnityContainer container)
        {
            this.container = container;
        }

        public void Log(string message, Category category, Priority priority)
        {
            Trace.WriteLine(message, category.ToString());
            try
            {
                var aggregator = container.Resolve<IEventAggregator>();

                var data = new LogMessage();
                data.Message = message;
                data.Category = category;
                data.Priority = priority;

                aggregator.GetEvent<LogEvent>().Publish(data);
            }
            catch (Exception)
            {
            }
        }
    }
}