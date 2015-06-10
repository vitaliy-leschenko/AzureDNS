namespace AzureDNS.Core.DnsReaders
{
    public class MxDnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var mx = new MxDnsRecord();
            mx.Exchange = data.Exchange;
            mx.Preference = data.Preference;

            return mx;
        }
    }
}