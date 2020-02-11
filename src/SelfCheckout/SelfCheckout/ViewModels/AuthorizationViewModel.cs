using SelfCheckout.Resources;
using SelfCheckout.Validations;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class AuthorizationViewModel : ViewModelBase
    {
        TaskCompletionSource<bool> _task;

        ValidatableObject<string> _userName;
        ValidatableObject<string> _password;

        public AuthorizationViewModel()
        {
            _userName = new ValidatableObject<string>();
            _password = new ValidatableObject<string>();

            AddValidation();
        }

        public ICommand ValidateUserNameCommand => new Command(() => ValidateUserName());

        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());

        public ICommand ConfirmCommand => new Command(async() =>
        {
            if(ValidateUserName() && ValidatePassword())
            {
                _task?.SetResult(true);
                await NavigationService.PopBackAsync();
            }
        });

        public ICommand CancelCommand => new Command(async() =>
        {
            _task?.SetResult(false);
            await NavigationService.PopBackAsync();
        });

        public override Task InitializeAsync<TViewModel, TResult>(object param, TaskCompletionSource<TResult> task)
        {
            _task = task as TaskCompletionSource<bool>;

            return base.InitializeAsync<TViewModel, TResult>(param, task);
        }

        public ValidatableObject<string> UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public ValidatableObject<string> Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        bool ValidateUserName()
        {
            return UserName.Validate();
        }

        bool ValidatePassword()
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
