using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Identity;
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
        public ICommand SettingCommand => new Command(async () =>
        {
            var task = new TaskCompletionSource<bool>();
            await NavigationService.PushModalAsync<AuthorizationViewModel, bool>(null, task);
            var result = await task.Task;
            if (result)
                await NavigationService.NavigateToAsync<SettingViewModel>();
        });

        public ICommand ConfirmCommand => new Command(async () => await LoginAsync());

        async Task LoginAsync()
        {
            if (ValidateUserName() && ValidatePassword())
            {
                try
                {
                    IsBusy = true;

                    var payload = new
                    {
                        branch_no = "40",
                        module_code = "MpKpi",
                        user_code = UserName.Value,
                        user_password = Password.Value,
                        machine_ip = "127.0.0.1"
                    };
                    var loginResult = await IdentityService.LoginAsync(payload);
                    if (loginResult.IsCompleted)
                    {
                        IdentityService.LoginData = loginResult.Data;
                        await NavigationService.NavigateToAsync<BorrowViewModel>();
                    }
                    else
                    {
                        Password.Errors = new List<string>() { loginResult.DefaultMessage };
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
        }
    }
}
