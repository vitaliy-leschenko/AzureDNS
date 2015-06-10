namespace AzureDNS.Core
{
    public class TxtDnsRecord : BaseDnsRecord
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}