namespace AzureDNS.Core
{
    public class NsDnsRecord : BaseDnsRecord
    {
        public string Nsdname { get; set; }

        public override string ToString()
        {
            return Nsdname;
        }
    }
}