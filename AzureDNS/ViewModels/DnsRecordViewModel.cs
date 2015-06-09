using System.Collections.Generic;
using AzureDNS.Common;

namespace AzureDNS.ViewModels
{
    public class DnsRecordViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string RecordType { get; set; }
        public List<object> Records { get; set; }
    }
}