namespace AzureDNS.Core.DnsReaders
{
    public class ADnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var a = new ADnsRecord();
            a.Ipv4Address = data.Ipv4Address;

            return a;
        }
    }
}