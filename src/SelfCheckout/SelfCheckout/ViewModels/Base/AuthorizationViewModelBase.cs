using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.Validations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class AuthorizationViewModelBase : NavigatableViewModelBase
    {
        protected ISaleEngineService SaleEngineService { get; private set; }
        protected ISelfCheckoutService SelfCheckoutService { get; private set; }

        ValidatableObject<string> _userName;
        ValidatableObject<string> _password;

        public AuthorizationViewModelBase(INavigationService navigationService, ISaleEngineService saleEngineService, 
            ISelfCheckoutService selfCheckoutService) : base(navigationService)
        {
            SaleEngineService = saleEngineService;
            SelfCheckoutService = selfCheckoutService;

            _userName = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            AddValidation();
        }

        public ICommand ValidateUserNameCommand => new Command(() => ValidateUserName());

        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());

        public ICommand AuthorizeCommand => new DelegateCommand(async () =>
        {
            if (ValidateUserName() && ValidatePassword())
            {
                try
                {
                    IsBusy = true;
                    var result = await SaleEngineService.LoginAsync(UserName.Value, Password.Value);
                    await AuthorizeCallback(result);
                }
                catch (Exception ex)
                {
                    Password.Errors = new List<string>() { ex.Message };
                }
                finally
                {
                    IsBusy = false;
                }
            }
        });

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

        protected virtual Task AuthorizeCallback(LoginData loginData)
        {
            return Task.FromResult(false);
        }
    }
}
