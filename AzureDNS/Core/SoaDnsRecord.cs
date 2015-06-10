namespace AzureDNS.Core
{
    public class SoaDnsRecord : BaseDnsRecord
    {
        public string Host { get; set; }
        public string Email { get; set; }
        public uint SerialNumber { get; set; }
        public uint RefreshTime { get; set; }
        public uint RetryTime { get; set; }
        public uint ExpireTime { get; set; }
        public uint MinimumTtl { get; set; }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2},{3},{4},{5}]", Host, Email, RefreshTime, RetryTime, ExpireTime, MinimumTtl);
        }
    }
}