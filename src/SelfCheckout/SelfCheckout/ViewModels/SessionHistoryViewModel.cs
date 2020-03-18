using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.ViewModels
{
    public class SessionHistoryViewModel : ViewModelBase
    {
        public SessionHistoryViewModel(INavigationService navigatinService, IDialogService dialogService) : base(navigatinService, dialogService)
        {
        }
    }
}
