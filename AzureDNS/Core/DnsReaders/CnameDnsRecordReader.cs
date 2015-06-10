namespace AzureDNS.Core.DnsReaders
{
    public class CnameDnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var cname = new CnameDnsRecord();
            cname.Cname = data.Cname;

            return cname;
        }
    }
}