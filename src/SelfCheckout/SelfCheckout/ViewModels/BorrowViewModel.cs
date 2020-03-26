using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
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
    public class BorrowViewModel : ShoppingCartViewModelBase
    {
        string _inputValue = "3600000711400";

        public BorrowViewModel(INavigationService navigatinService, IDialogService dialogService, ISaleEngineService saleEngineService, ISelfCheckoutService selfCheckoutService, IRegisterService registerService) : base(navigatinService, dialogService, saleEngineService, selfCheckoutService, registerService)
        {
        }

        public ICommand ScanShoppingCardCommand => new DelegateCommand(() => ScanShoppingCard());

        public ICommand ValidateShoppingCardCommand => new DelegateCommand(async () => await ValidateShoppingCardAsync(InputValue));

        public string InputValue
        {
            get => _inputValue;
            set => SetProperty(ref _inputValue, value);
        }

        protected override Task ScanShoppingCardCallback(string inputValue)
        {
            InputValue = inputValue;
            return Task.FromResult(true);
        }

        protected override async Task ValidateShoppingCardCallback(string shoppingCard)
        {
            try
            {
                IsBusy = true;
                var userInfo = SaleEngineService.LoginData?.UserInfo;
                await SelfCheckoutService.StartSessionAsync(userInfo.UserCode, userInfo.MachineEnv.MachineNo, shoppingCard);
                await NavigationService.NavigateAsync("MainView");
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
