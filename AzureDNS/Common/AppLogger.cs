using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AzureDNS.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

namespace AzureDNS.Common
{
    public class LogViewer{}

    class AppLogger : ILoggerFacade
    {
        private readonly IUnityContainer container;
        private readonly Queue<LogMessage> failedMessages = new Queue<LogMessage>();

        public AppLogger(IUnityContainer container)
        {
            this.container = container;
        }

        public void Log(string message, Category category, Priority priority)
        {
            lock (this)
            {
                Trace.WriteLine(message, category.ToString());
                var data = new LogMessage();
                data.Message = message;
                data.Category = category;
                data.Priority = priority;

                try
                {
                    if (!container.IsRegistered<LogViewer>()) throw new NotImplementedException();

                    var aggregator = container.Resolve<IEventAggregator>();
                    while (failedMessages.Any())
                    {
                        var item = failedMessages.Dequeue();
                        aggregator.GetEvent<LogEvent>().Publish(item);
                    }
                    aggregator.GetEvent<LogEvent>().Publish(data);
                }
                catch (Exception)
                {
                    failedMessages.Enqueue(data);
                }
            }
        }
    }
}