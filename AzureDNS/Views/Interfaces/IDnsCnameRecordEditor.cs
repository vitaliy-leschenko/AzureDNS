namespace AzureDNS.Views.Interfaces
{
    public interface IDnsCnameRecordEditor : IDnsRecordEditor
    {
        void FocusHostName();
        void FocusCname();
    }
}