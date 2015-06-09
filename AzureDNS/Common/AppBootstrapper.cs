using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Windows;
using AzureDNS.Views;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace AzureDNS.Common
{
    class AppBootstrapper: UnityBootstrapper
    {
        public AppBootstrapper(App app)
        {
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);
            if (Shell == null) Application.Current.Shutdown();
        }

        protected override DependencyObject CreateShell()
        {
            var ps = Container.Resolve<AzurePowerShell>();
            //if (!ps.AddAzureAccount()) return null;

            ps.InitializeAzureResourceManager();

            return Container.Resolve<SubscriptionsView>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            ((Window)Shell).Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            var iss = InitialSessionState.CreateDefault();
            var rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();

            Container.RegisterInstance(rs);
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new AppLogger();
        }
    }
}
