using System.Management.Automation.Runspaces;
using System.Windows;
using AzureDNS.Core;
using AzureDNS.Core.DnsReaders;
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
                .RegisterType<IDnsRecordReader, TxtDnsRecordReader>("TXT");

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
