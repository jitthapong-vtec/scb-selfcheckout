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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class BorrowViewModel : ViewModelBase
    {
        string _inputValue = "3600000711400";

        public BorrowViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
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
                var validateResult = await SelfCheckoutService.ValidateShoppingCartAsync(LoginData.UserInfo.MachineEnv.MachineIp, InputValue);
                if (!validateResult.IsCompleted)
                {
                    DialogService.ShowAlert(AppResources.Opps, validateResult.DefaultMessage, AppResources.Close);
                    return;
                }

                SelfCheckoutService.CurrentShoppingCart = InputValue;

                var payload = new
                {
                    shoppingCard = InputValue,
                    SubBranch = AppConfig.SubBranch,
                    isTour = false,
                    isGenPdfPromotion = false,
                    isGenImgShoppingCard = false
                };

                await RegisterService.GetCustomerAsync(payload);

                if (CustomerData?.Person != null)
                {
                    //if (!CustomerData.Person.IsActivate)
                    //{
                    //    await DialogService.ShowAlertAsync(AppResources.Opps, $"{CustomerData.Person.NativeName} is not activate!", AppResources.Close);
                    //    return;
                    //}
                    var parameters = new DialogParameters()
                    {
                        {"Person", CustomerData.Person }
                    };
                    DialogService.ShowDialog("CustomerConfirmDialog", parameters, async (dialogResult) =>
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
                var startResult = await SelfCheckoutService.StartSessionAsync(LoginData.UserInfo.UserCode, LoginData.UserInfo.MachineEnv.MachineNo, InputValue);
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
