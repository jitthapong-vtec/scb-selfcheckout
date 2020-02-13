﻿using SelfCheckout.Resources;
using SelfCheckout.Validations;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class AuthorizationViewModel : AuthorizationViewModelBase
    {
        TaskCompletionSource<bool> _task;

        public ICommand ConfirmCommand => new Command(async () =>
        {
            if (ValidateUserName() && ValidatePassword())
            {
                try
                {
                    IsBusy = true;
                    var loginResult = await IdentityService.LoginAsync("40", "MpKpi", "127.0.0.1", UserName.Value, Password.Value);
                    if (loginResult.IsCompleted)
                    {
                        await NavigationService.PopModalAsync();
                        _task?.SetResult(true);
                    }
                    else
                    {
                        Password.Errors = new List<string>() { loginResult.Message.FirstOrDefault()?.MessageDesc };
                    }
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

        public ICommand CancelCommand => new Command(async () =>
        {
            await NavigationService.PopModalAsync();
            _task?.SetResult(false);
        });

        public override Task InitializeAsync<TViewModel, TResult>(object param, TaskCompletionSource<TResult> task)
        {
            _task = task as TaskCompletionSource<bool>;

            return base.InitializeAsync<TViewModel, TResult>(param, task);
        }
    }
}