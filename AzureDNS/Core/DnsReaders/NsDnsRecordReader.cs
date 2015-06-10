namespace AzureDNS.Core.DnsReaders
{
    public class NsDnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var ns = new NsDnsRecord();
            ns.Nsdname = data.Nsdname;

            return ns;
        }
    }
}