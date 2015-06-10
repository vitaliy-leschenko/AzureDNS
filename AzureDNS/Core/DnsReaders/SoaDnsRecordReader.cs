namespace AzureDNS.Core.DnsReaders
{
    public class SoaDnsRecordReader : IDnsRecordReader
    {
        public BaseDnsRecord Read(dynamic data)
        {
            var soa = new SoaDnsRecord();
            soa.Host = data.Host;
            soa.Email = data.Email;
            soa.SerialNumber = data.SerialNumber;
            soa.RefreshTime = data.RefreshTime;
            soa.RetryTime = data.RetryTime;
            soa.ExpireTime = data.ExpireTime;
            soa.MinimumTtl = data.MinimumTtl;

            return soa;
        }
    }
}