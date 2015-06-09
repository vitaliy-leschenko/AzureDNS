using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using AzureDNS.Common;

namespace AzureDNS
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var bootstrapper = new AppBootstrapper(this);
            bootstrapper.Run();
        }
    }
}
