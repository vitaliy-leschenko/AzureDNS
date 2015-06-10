namespace AzureDNS.Core
{
    public class SrvDnsRecord : BaseDnsRecord
    {
        public string Target { get; set; }
        public ushort Weight { get; set; }
        public ushort Port { get; set; }
        public ushort Priority { get; set; }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2},{3}]", Priority, Weight, Port, Target);
        }
    }
}