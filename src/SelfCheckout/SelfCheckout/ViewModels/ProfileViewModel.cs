using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        public ProfileViewModel(IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public ICommand ShowProgressCommand => new DelegateCommand(() => IsBusy = true);

        public ICommand HideProgressCommand => new DelegateCommand(() => IsBusy = false);

        public string MemberWebUrl {
            get => string.Format(_selfCheckoutService.AppConfig.UrlMemberWeb, _selfCheckoutService.CurrentShoppingCard, _selfCheckoutService.AppConfig.SubBranch);
        }
    }
}
