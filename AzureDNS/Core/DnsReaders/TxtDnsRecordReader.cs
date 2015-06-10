namespace AzureDNS.Core.DnsReaders
{
    public class TxtDnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var txt = new TxtDnsRecord();
            txt.Value = data.Value;

            return txt;
        }
    }
}