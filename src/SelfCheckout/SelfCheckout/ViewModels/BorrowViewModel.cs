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
        object _lock = new object();

        string _inputValue = "";

        bool _isBeingScan;

        public BorrowViewModel(INavigationService navigationService, ISaleEngineService saleEngineService,
            ISelfCheckoutService selfCheckoutService, IRegisterService registerService) :
            base(navigationService, saleEngineService, selfCheckoutService, registerService)
        {
        }

        public ICommand ScanCommand => new Command<object>((data) =>
        {
            InputValue = DecodeShoppingCardData(data?.ToString());
        });

        public ICommand ScanShoppingCardCommand => new Command(async () =>
        {
            lock (_lock)
            {
                if (IsBegingScan)
                    return;
                else
                    IsBegingScan = true;
            }
            var result = await NavigationService.ShowDialogAsync<string>("CameraScannerView", null);
            InputValue = result;
        });

        public ICommand ValidateShoppingCardCommand => new DelegateCommand(async () => await ValidateShoppingCardAsync(InputValue));

        public string InputValue
        {
            get => _inputValue;
            set => SetProperty(ref _inputValue, value);
        }

        public bool IsBegingScan
        {
            get => _isBeingScan;
            set => SetProperty(ref _isBeingScan, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsBegingScan = false;
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
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
