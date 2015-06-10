namespace AzureDNS.Core.DnsReaders
{
    public class SrvDnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var srv = new SrvDnsRecord();
            srv.Target = data.Target;
            srv.Weight = data.Weight;
            srv.Port = data.Port;
            srv.Priority = data.Priority;

            return srv;
        }
    }
}