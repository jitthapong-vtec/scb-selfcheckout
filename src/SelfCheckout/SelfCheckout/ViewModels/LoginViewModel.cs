using SelfCheckout.Models;
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
        IIdentityService _identityService;

        public LoginViewModel(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public ICommand ConfirmCommand => new Command(async () => await LoginAsync());

        async Task LoginAsync()
        {
            try
            {
                IsBusy = true;
                var userInput = new UserInput()
                {
                    BranchNo = "40",
                    ModuleCode = "MpKpi",
                    UserCode = "k",
                    UserPassword = "kkkk",
                    MachineIp = "127.0.0.1"
                };

                await _identityService.LoginAsync(userInput);
                await NavigationService.NavigateToAsync<BorrowViewModel>();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync("", ex.Message);
            }
        }
    }
}
