using Prism.Commands;
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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class LoginViewModel : AuthorizationViewModelBase, IInitialize, INavigatedAware
    {
        INavigationService _navigationService;
        IDialogService _dialogService;

        public LoginViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService) : base(saleEngineService, selfCheckoutService)
        {
            _navigationService = navigatinService;
            _dialogService = dialogService;
        }

        public string Version { get => VersionTracking.CurrentVersion; }

        public ICommand SettingCommand => new DelegateCommand(() =>
        {
            _dialogService.ShowDialog("AuthorizeDialog", null, async (result) =>
            {
                if (result.Parameters.GetValue<bool>("IsAuthorized"))
                {
                    var loginData = result.Parameters.GetValue<LoginData>("LoginData");
                    var navParam = new NavigationParameters()
                    {
                        {"LoginData", loginData }
                    };
                    await _navigationService.NavigateAsync("SettingView", navParam);
                }
            });
        });

        public ICommand CheckerSettingCommand => new DelegateCommand(async() =>
        {
            await _navigationService.NavigateAsync("CheckerSettingView");
        });

        public void Initialize(INavigationParameters parameters)
        {
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        protected override async Task AuthorizeCallback(LoginData loginData)
        {
            SaleEngineService.LoginData = loginData;
            if (Device.Idiom == TargetIdiom.Phone)
            {
                var result = await _navigationService.NavigateAsync("BorrowView");
            }
            else if (Device.Idiom == TargetIdiom.Desktop)
            {
                var result = await _navigationService.NavigateAsync("CheckerMainView");
            }
        }
    }
}
