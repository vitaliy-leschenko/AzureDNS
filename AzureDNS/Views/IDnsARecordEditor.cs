namespace AzureDNS.Views
{
    public interface IDnsARecordEditor : IDnsRecordEditor
    {
        void FocusHostName();
        void Complete();
    }
}