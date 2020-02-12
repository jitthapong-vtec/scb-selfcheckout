﻿using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Identity;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        string _userCode = "k";
        string _userPassword = "kkkk";

        public ICommand SettingCommand => new Command(async () => await NavigationService.NavigateToAsync<SettingViewModel>());

        public ICommand ConfirmCommand => new Command(async () => await LoginAsync());

        async Task LoginAsync()
        {
            try
            {
                IsBusy = true;
                var userInput = new
                {
                    branch_no = "40",
                    module_code = "MpKpi",
                    //BranchNo = AppConfig.BranchNo,
                    //ModuleCode = AppConfig.Module,
                    user_code = UserCode,
                    user_password = UserPassword,
                    machine_ip = "127.0.0.1"
                };

                var loginResult = await IdentityService.LoginAsync(userInput);
                if (loginResult.IsCompleted)
                {
                    await NavigationService.NavigateToAsync<BorrowViewModel>();
                }
                else
                {
                    await DialogService.ShowAlertAsync(AppResources.StaffLogin, loginResult.DefaultMessage);
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync("", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public string UserCode
        {
            get => _userCode;
            set
            {
                _userCode = value;
                RaisePropertyChanged(() => UserCode);
            }
        }

        public string UserPassword
        {
            get => _userPassword;
            set
            {
                _userPassword = value;
                RaisePropertyChanged(() => UserPassword);
            }
        }
    }
}
