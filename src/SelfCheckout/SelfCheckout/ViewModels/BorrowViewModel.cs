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
    public class BorrowViewModel : ShoppingCartViewModelBase, INavigationAware
    {
        object _lock = new object();

        string _inputValue = "";

        bool _isBeingScan;

        public BorrowViewModel(INavigationService navigationService, IDialogService dialogService, ISaleEngineService saleEngineService,
            ISelfCheckoutService selfCheckoutService, IRegisterService registerService) :
            base(dialogService, saleEngineService, selfCheckoutService, registerService)
        {
            NavigationService = navigationService;
        }

        public ICommand ScanCommand => new Command<object>((data) =>
        {
            InputValue = data?.ToString();
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
            await NavigationService.NavigateAsync("CameraScannerView");
        });

        public ICommand ValidateShoppingCardCommand => new DelegateCommand(async () => await ValidateShoppingCardAsync(InputValue));

        public INavigationService NavigationService { get; private set; }

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

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var scanData = parameters.GetValue<string>("ScanData");
            if (!string.IsNullOrEmpty(scanData))
            {
                InputValue = scanData;
            }
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
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
