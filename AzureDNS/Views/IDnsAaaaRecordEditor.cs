namespace AzureDNS.Views
{
    public interface IDnsAaaaRecordEditor : IDnsRecordEditor
    {
        void FocusHostName();
        void Complete();
    }
}