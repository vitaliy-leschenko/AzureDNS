using System.Collections.Generic;
using System.Text;
using AzureDNS.Common;
using AzureDNS.Core;

namespace AzureDNS.ViewModels
{
    public class DnsRecordViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public RecordType RecordType { get; set; }
        public List<BaseDnsRecord> Records { get; set; }

        public string RecordsValue
        {
            get
            {
                var b = new StringBuilder();
                foreach (var record in Records)
                {
                    if (b.Length > 0) b.AppendLine();
                    b.Append(record);
                }
                return b.ToString();
            }
        }

        public bool AllowEdit
        {
            get
            {
                return RecordType == RecordType.A ||
                       RecordType == RecordType.AAAA ||
                       RecordType == RecordType.CNAME ||
                       RecordType == RecordType.MX ||
                       RecordType == RecordType.TXT ||
                       RecordType == RecordType.NS;
            }
        }

        public bool AllowDelete
        {
            get
            {
                return RecordType == RecordType.A ||
                       RecordType == RecordType.AAAA ||
                       RecordType == RecordType.CNAME ||
                       RecordType == RecordType.MX ||
                       RecordType == RecordType.TXT;
            }
        }
    }
}