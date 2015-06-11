namespace AzureDNS.Views
{
    public interface IDnsTxtRecordEditor : IDnsRecordEditor
    {
        void FocusHostName();
        void Complete();
    }
}