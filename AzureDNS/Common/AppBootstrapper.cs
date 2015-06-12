using System.Management.Automation.Runspaces;
using System.Windows;
using AzureDNS.Core;
using AzureDNS.Core.DnsReaders;
using AzureDNS.Core.PS;
using AzureDNS.Views;
using AzureDNS.Views.Interfaces;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

namespace AzureDNS.Common
{
    class AppBootstrapper: UnityBootstrapper
    {
        private readonly IUnityContainer container;

        public AppBootstrapper(App app)
        {
            container = new UnityContainer();
        }

        protected override IUnityContainer CreateContainer()
        {
            return container;
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);
            if (Shell == null) Application.Current.Shutdown();
        }

        protected override DependencyObject CreateShell()
        {
            var ps = container.Resolve<AzurePowerShell>();
            ps.InitializeAzureResourceManager();

            var manager = Container.Resolve<IRegionManager>();
            manager.RegisterViewWithRegion("Logs", () => Container.Resolve<LogsView>());
            manager.RegisterViewWithRegion("Subscriptions", () => Container.Resolve<SubscriptionsView>());
            manager.RegisterViewWithRegion("DnsZones", () => Container.Resolve<DnsZonesView>());
            manager.RegisterViewWithRegion("DnsRecords", () => Container.Resolve<DnsRecordsView>());

            return Container.Resolve<MainPageView>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            ((Window)Shell).Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container
                .RegisterType<IDnsRecordReader, ADnsRecordReader>("A")
                .RegisterType<IDnsRecordReader, AaaaDnsRecordReader>("AAAA")
                .RegisterType<IDnsRecordReader, CnameDnsRecordReader>("CNAME")
                .RegisterType<IDnsRecordReader, MxDnsRecordReader>("MX")
                .RegisterType<IDnsRecordReader, NsDnsRecordReader>("NS")
                .RegisterType<IDnsRecordReader, SoaDnsRecordReader>("SOA")
                .RegisterType<IDnsRecordReader, SrvDnsRecordReader>("SRV")
                .RegisterType<IDnsRecordReader, TxtDnsRecordReader>("TXT")
                .RegisterType<IDnsRecordEditor, DnsARecordEditorView>("A")
                .RegisterType<IDnsRecordEditor, DnsAaaaRecordEditorView>("AAAA")
                .RegisterType<IDnsRecordEditor, DnsCnameRecordEditorView>("CNAME")
                .RegisterType<IDnsRecordEditor, DnsTxtRecordEditorView>("TXT")
                .RegisterType<IDnsRecordEditor, DnsMxRecordEditorView>("MX");

            var host = Container.Resolve<AppPSHost>();
            var iss = InitialSessionState.CreateDefault();
            var rs = RunspaceFactory.CreateRunspace(host, iss);
            rs.Open();

            Container.RegisterInstance(rs);
        }

        protected override ILoggerFacade CreateLogger()
        {
            return container.Resolve<AppLogger>();
        }
    }
}
