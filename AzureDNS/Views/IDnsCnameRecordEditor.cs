namespace AzureDNS.Views
{
    public interface IDnsCnameRecordEditor : IDnsRecordEditor
    {
        void FocusHostName();
        void FocusCname();
        void Complete();
    }
}