namespace AzureDNS.Core
{
    public class CnameDnsRecord : BaseDnsRecord
    {
        public string Cname { get; set; }

        public override string ToString()
        {
            return Cname;
        }
    }
}