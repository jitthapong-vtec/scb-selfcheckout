using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class LoginViewModel : AuthorizationViewModelBase
    {
        public LoginViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService, 
            ISaleEngineService saleEngineService) : base(navigationService, saleEngineService, selfCheckoutService)
        {
        }

        public ICommand SettingCommand => new DelegateCommand(async() =>
        {
            var result = await NavigationService.ShowDialogAsync<INavigationParameters>("AuthorizeDialog", null);
            if (result.GetValue<bool>("IsAuthorized"))
            {
                var loginData = result.GetValue<LoginData>("LoginData");
                var navParam = new NavigationParameters()
                    {
                        {"LoginData", loginData }
                    };
                await NavigationService.NavigateAsync("SettingView", navParam);
            }
        });

        public ICommand CheckerSettingCommand => new DelegateCommand(async() =>
        {
            await NavigationService.NavigateAsync("CheckerSettingView");
        });

        protected override async Task AuthorizeCallback(LoginData loginData)
        {
            SaleEngineService.LoginData = loginData;
            if (Device.Idiom == TargetIdiom.Phone)
            {
                var result = await NavigationService.NavigateAsync("BorrowView");
            }
            else if (Device.Idiom == TargetIdiom.Desktop)
            {
                var result = await NavigationService.NavigateAsync("CheckerMainView");
            }
        }
    }
}
