using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
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
        string _loginTitleText;
        string _usernameLabel;
        string _passwordLabel;
        string _confirmBtnText;
        string _settingLabel;

        public LoginViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService, 
            ISaleEngineService saleEngineService) : base(navigationService, saleEngineService, selfCheckoutService)
        {
            SetupLanguage();
        }

        private void SetupLanguage()
        {
            LoginTitleText = AppResources.StaffLogin;
            UsernameLabel = AppResources.UserName;
            PasswordLabel = AppResources.Password;
            ConfirmBtnText = AppResources.Confirm;
            SettingLabel = AppResources.Setting;
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

        public string LoginTitleText {
            get => _loginTitleText;
            set => SetProperty(ref _loginTitleText, value);
        }

        public string UsernameLabel
        {
            get => _usernameLabel;
            set => SetProperty(ref _usernameLabel, value);
        }

        public string PasswordLabel
        {
            get => _passwordLabel;
            set => SetProperty(ref _passwordLabel, value);
        }

        public string ConfirmBtnText
        {
            get => _confirmBtnText;
            set => SetProperty(ref _confirmBtnText, value);
        }

        public string SettingLabel
        {
            get => _settingLabel;
            set => SetProperty(ref _settingLabel, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Languages = SelfCheckoutService.Languages.ToObservableCollection();
            LanguageSelected = SelfCheckoutService.CurrentLanguage;
        }

        protected override Task OnLanguageChanged(Language lang)
        {
            SetupLanguage();
            return base.OnLanguageChanged(lang);
        }

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
