using SelfCheckout.Models;
using SelfCheckout.Services.Authen;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        ILoginService _loginService;

        public LoginViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public ICommand ConfirmCommand => new Command(async () => await LoginAsync());

        async Task LoginAsync()
        {
            try
            {
                var userInput = new UserInput()
                {
                    BranchNo = "40",
                    ModuleCode = "MpKpi",
                    UserCode = "k",
                    UserPassword = "kkkk",
                    MachineIp = "127.0.0.1"
                };

                var loginUser = await _loginService.LoginAsync(userInput);
                await NavigationService.NavigateToAsync<BorrowViewModel>();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
