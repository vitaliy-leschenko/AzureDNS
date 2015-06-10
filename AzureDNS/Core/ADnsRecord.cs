namespace AzureDNS.Core
{
    public class ADnsRecord : BaseDnsRecord
    {
        public string Ipv4Address { get; set; }

        public override string ToString()
        {
            return Ipv4Address;
        }
    }
}