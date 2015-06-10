namespace AzureDNS.Core
{
    public class MxDnsRecord : BaseDnsRecord
    {
        public ushort Preference { get; set; }
        public string Exchange { get; set; }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", Preference, Exchange);
        }
    }
}