namespace AzureDNS.Core
{
    public class AaaaDnsRecord : BaseDnsRecord
    {
        public string Ipv6Address { get; set; }

        public override string ToString()
        {
            return Ipv6Address;
        }
    }
}