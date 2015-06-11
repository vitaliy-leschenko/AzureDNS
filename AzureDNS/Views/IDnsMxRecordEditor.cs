namespace AzureDNS.Views
{
    public interface IDnsMxRecordEditor : IDnsRecordEditor
    {
        void FocusHostName();
        void Complete();
    }
}