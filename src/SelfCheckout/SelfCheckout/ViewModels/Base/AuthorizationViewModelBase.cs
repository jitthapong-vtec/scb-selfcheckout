using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.Validations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class AuthorizationViewModelBase : ViewModelBase
    {
        ValidatableObject<string> _userName;
        ValidatableObject<string> _password;

        public AuthorizationViewModelBase(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
            _userName = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            _userName.Value = "k";
            _password.Value = "kkkk";
            AddValidation();
        }

        public ICommand ValidateUserNameCommand => new Command(() => ValidateUserName());

        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());

        public ValidatableObject<string> UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public ValidatableObject<string> Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        protected bool ValidateUserName()
        {
            return UserName.Validate();
        }

        protected bool ValidatePassword()
        {
            return Password.Validate();
        }

        void AddValidation()
        {
            _userName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = AppResources.PleaseEnterUserName });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = AppResources.PleaseEnterPassword });
        }
    }
}
