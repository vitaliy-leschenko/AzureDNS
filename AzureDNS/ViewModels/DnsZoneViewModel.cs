using AzureDNS.Common;

namespace AzureDNS.ViewModels
{
    public class DnsZoneViewModel: BaseViewModel
    {
        public string Name { get; set; }
        public string ResourceGroupName { get; set; }
    }
}
