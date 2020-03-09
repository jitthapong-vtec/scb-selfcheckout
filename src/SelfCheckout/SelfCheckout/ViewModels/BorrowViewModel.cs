using Newtonsoft.Json;
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
    public class BorrowViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;
        ISaleEngineService _saleEngineService;
        IRegisterService _registerService;

        string _inputValue = "3600000711400";

        public BorrowViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
            _saleEngineService = saleEngineService;
            _registerService = registerService;
        }

        public ICommand ScanShoppingCartCommand => new Command(() => ScanShoppingCart());

        public ICommand ValidateShoppingCartCommand => new Command(async () => await ValidateShoppingCartAsync());

        void ScanShoppingCart()
        {
            DialogService.ShowDialog("BarcodeScanDialog", null, (scanResult) =>
            {
                var result = scanResult.Parameters.GetValue<string>("ScanData");
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        var definition = new { S = "", C = "" };
                        var qrFromKiosk = JsonConvert.DeserializeAnonymousType(result, definition);
                        InputValue = qrFromKiosk.S;
                    }
                    catch
                    {
                        InputValue = result;
                    }
                }
            });
        }

        async Task ValidateShoppingCartAsync()
        {
            try
            {
                IsBusy = true;
                var validateResult = await _selfCheckoutService.ValidateShoppingCartAsync(_saleEngineService.LoginData.UserInfo.MachineEnv.MachineIp, InputValue);
                if (!validateResult.IsCompleted)
                {
                    DialogService.ShowAlert(AppResources.Opps, validateResult.DefaultMessage, AppResources.Close);
                    return;
                }

                _selfCheckoutService.CurrentShoppingCart = InputValue;

                var payload = new
                {
                    shoppingCard = InputValue,
                    SubBranch = _selfCheckoutService.AppConfig.SubBranch,
                    isTour = false,
                    isGenPdfPromotion = false,
                    isGenImgShoppingCard = false
                };

                var customersData = await _registerService.GetCustomerAsync(payload);

                var customerData = customersData.FirstOrDefault();
                if (customerData?.Person != null)
                {
                    //if (!CustomerData.Person.IsActivate)
                    //{
                    //    await DialogService.ShowAlertAsync(AppResources.Opps, $"{CustomerData.Person.NativeName} is not activate!", AppResources.Close);
                    //    return;
                    //}
                    var parameters = new DialogParameters()
                    {
                        {"Person", customerData.Person }
                    };
                    DialogService.ShowDialog("CustomerCartConfirmDialog", parameters, async (dialogResult) =>
                    {
                        var isConfirm = dialogResult.Parameters.GetValue<bool>("IsConfirm");
                        if (isConfirm)
                            await StartSessionAsync();
                    });
                }
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task StartSessionAsync()
        {
            try
            {
                IsBusy = true;
                var userInfo = _saleEngineService.LoginData?.UserInfo;
                var startResult = await _selfCheckoutService.StartSessionAsync(userInfo.UserCode, userInfo.MachineEnv.MachineNo, InputValue);
                if (!startResult.IsCompleted)
                {
                    DialogService.ShowAlert(AppResources.Opps, startResult.DefaultMessage, AppResources.Close);
                    return;
                }
                await NavigationService.NavigateAsync("MainView");
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public string InputValue
        {
            get => _inputValue;
            set => SetProperty(ref _inputValue, value);
        }
    }
}
