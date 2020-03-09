using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class LoginViewModel : AuthorizationViewModelBase
    {
        INavigationService _navigationService;
        IDialogService _dialogService;

        public LoginViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService) : base(saleEngineService, selfCheckoutService)
        {
            _navigationService = navigatinService;
            _dialogService = dialogService;
        }

        public ICommand SettingCommand => new Command(() =>
        {
            _dialogService.ShowDialog("AuthorizeDialog", null, async (result) =>
            {
                if (result.Parameters.GetValue<bool>("IsAutorized"))
                    await _navigationService.NavigateAsync("SettingView");
            });
        });

        protected override async Task AuthorizeCallback(LoginData loginData)
        {
            SaleEngineService.LoginData = loginData;
            if (Device.Idiom == TargetIdiom.Phone)
                await _navigationService.NavigateAsync("BorrowView");
            else if (Device.Idiom == TargetIdiom.Desktop)
                await _navigationService.NavigateAsync("CheckerMainView");
        }
    }
}
