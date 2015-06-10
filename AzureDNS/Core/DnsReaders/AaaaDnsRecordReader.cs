namespace AzureDNS.Core.DnsReaders
{
    public class AaaaDnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var aaaa = new AaaaDnsRecord();
            aaaa.Ipv6Address = data.Ipv6Address;

            return aaaa;
        }
    }
}