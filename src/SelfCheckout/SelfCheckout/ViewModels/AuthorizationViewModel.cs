using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
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
    public class AuthorizationViewModel : AuthorizationViewModelBase, IDialogAware
    {
        public AuthorizationViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public ICommand ConfirmCommand => new Command(() =>
        {
            if (ValidateUserName() && ValidatePassword())
            {
                try
                {
                    IsBusy = true;
                    //var payload = new
                    //{
                    //    branch_no = "40",
                    //    module_code = "MpKpi",
                    //    user_code = UserName.Value,
                    //    user_password = Password.Value,
                    //    machine_ip = "127.0.0.1"
                    //};
                    //var loginResult = await SaleEngineService.LoginAsync(payload);
                    //if (loginResult.IsCompleted)
                    //{
                    var parameters = new DialogParameters()
                    {
                        {"IsAuthorized", true }
                    };
                    RequestClose(parameters);
                    //}
                    //else
                    //{
                    //    Password.Errors = new List<string>() { loginResult.Message.FirstOrDefault()?.MessageDesc };
                    //}
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

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
