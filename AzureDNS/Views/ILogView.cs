using AzureDNS.Events;

namespace AzureDNS.Views
{
    public interface ILogView: IBaseView
    {
        void FocusItem(LogMessage item);
    }
}