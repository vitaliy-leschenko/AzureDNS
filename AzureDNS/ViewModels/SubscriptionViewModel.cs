using System;
using AzureDNS.Common;

namespace AzureDNS.ViewModels
{
    public class SubscriptionViewModel : BaseViewModel
    {
        public Guid SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
    }
}