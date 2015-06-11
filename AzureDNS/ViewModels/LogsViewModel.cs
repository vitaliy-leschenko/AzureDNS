using System;
using System.Collections.ObjectModel;
using System.Windows;
using AzureDNS.Common;
using AzureDNS.Events;
using AzureDNS.Views;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

namespace AzureDNS.ViewModels
{
    public class LogsViewModel: BaseViewModel
    {
        private readonly ILogView view;
        private readonly IUnityContainer container;
        private string logText;

        public LogsViewModel(ILogView view, IUnityContainer container)
        {
            this.view = view;
            this.container = container;

            view.Loaded += OnLoaded;
            view.Unloaded += OnUnloaded;
        }

        public string LogText
        {
            get { return logText; }
            set
            {
                logText = value;
                OnPropertyChanged();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var aggregator = container.Resolve<IEventAggregator>();
            aggregator.GetEvent<LogEvent>().Unsubscribe(OnLogEvent);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var aggregator = container.Resolve<IEventAggregator>();
            aggregator.GetEvent<LogEvent>().Subscribe(OnLogEvent, ThreadOption.UIThread);

            container.RegisterInstance(new LogViewer());
        }

        private void OnLogEvent(LogMessage item)
        {
            var text = string.Format("[{0}] {1}", item.Category, item.Message);
            if (string.IsNullOrEmpty(LogText))
            {
                LogText = text;
            }
            else
            {
                LogText += Environment.NewLine + text;
            }

            view.ScrollDown();
        }
    }
}
