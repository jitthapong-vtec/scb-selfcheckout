using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;
        string _memberWebUrl;

        public ProfileViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public string MemberWebUrl {
            get => string.Format(_selfCheckoutService.AppConfig.UrlMemberWeb, _selfCheckoutService.StartedShoppingCard, _selfCheckoutService.AppConfig.SubBranch);
        }
    }
}
