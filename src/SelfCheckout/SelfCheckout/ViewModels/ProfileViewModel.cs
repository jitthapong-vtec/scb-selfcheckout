using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;
        string _memberUrl;

        public ProfileViewModel(ISelfCheckoutService selfCheckoutService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public override Task OnTabSelected(TabItem item)
        {
            MemberWebUrl = string.Format(_selfCheckoutService.AppConfig.UrlMemberWeb, _selfCheckoutService.CurrentShoppingCard, _selfCheckoutService.AppConfig.SubBranch);
            return base.OnTabSelected(item);
        }

        public ICommand ShowProgressCommand => new DelegateCommand(() => IsBusy = true);

        public ICommand HideProgressCommand => new DelegateCommand(() => IsBusy = false);

        public string MemberWebUrl
        {
            get => _memberUrl;
            set => SetProperty(ref _memberUrl, value);
        }
    }
}
