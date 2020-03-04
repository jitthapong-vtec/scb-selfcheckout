using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
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
        public LoginViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public ICommand SettingCommand => new Command(async () =>
        {
            //var task = new TaskCompletionSource<bool>();
            //await NavigationService.PushModalAsync<AuthorizationViewModel, bool>(null, task);
            //var result = await task.Task;
            //if (result)
            //    await NavigationService.NavigateToAsync<SettingViewModel>();

            await NavigationService.NavigateAsync("SettingView");
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
                        branch_no = AppConfig.BranchNo,
                        module_code = AppConfig.Module,
                        user_code = UserName.Value,
                        user_password = Password.Value,
                        machine_ip = "127.0.0.1"
                    };
                    var result = await SaleEngineService.LoginAsync(payload);
                    SaleEngineService.LoginData = result.Data;

                    if (Device.Idiom == TargetIdiom.Phone)
                        await NavigationService.NavigateAsync("BorrowView");
                    else if (Device.Idiom == TargetIdiom.Desktop)
                        await NavigationService.NavigateAsync("CheckerMainView");
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
