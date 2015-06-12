using System;
using System.Deployment.Application;
using System.Globalization;
using System.Management.Automation.Host;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;

namespace AzureDNS.Core.PS
{
    public class AppPSHost: PSHost
    {
        private readonly IUnityContainer container;
        private readonly ILoggerFacade logger;
        private readonly PSHostUserInterface ui;

        public AppPSHost(IUnityContainer container)
        {
            this.container = container;
            logger = container.Resolve<ILoggerFacade>();
            ui = container.Resolve<PowerShellUI>();
        }

        public override void SetShouldExit(int exitCode)
        {
            Application.Current.Shutdown(exitCode);
        }

        public override void EnterNestedPrompt()
        {
        }

        public override void ExitNestedPrompt()
        {
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override string Name
        {
            get { return (string)Application.Current.Resources["appTitle"]; }
        }

        public override Version Version
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion;
                }
                return new Version(1, 0, 0, 0);
            }
        }

        public override Guid InstanceId
        {
            get { return Guid.NewGuid(); }
        }

        public override PSHostUserInterface UI
        {
            get { return ui; }
        }

        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }
    }
}
