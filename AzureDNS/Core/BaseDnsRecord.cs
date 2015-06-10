namespace AzureDNS.Core
{
    public class BaseDnsRecord
    {
    }

    public interface IDnsRecordReader
    {
        BaseDnsRecord Read(dynamic data);
    }
}
