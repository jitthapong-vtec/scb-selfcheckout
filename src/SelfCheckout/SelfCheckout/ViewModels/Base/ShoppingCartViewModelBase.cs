using Newtonsoft.Json;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ShoppingCartViewModelBase : ViewModelBase
    {
        protected ISaleEngineService SaleEngineService { get; private set; }
        protected ISelfCheckoutService SelfCheckoutService { get; private set; }
        protected IRegisterService RegisterService { get; private set; }

        public ShoppingCartViewModelBase(INavigationService navigatinService, IDialogService dialogService,
            ISaleEngineService saleEngineService, ISelfCheckoutService selfCheckoutService, IRegisterService registerService) : base(navigatinService, dialogService)
        {
            SaleEngineService = saleEngineService;
            SelfCheckoutService = selfCheckoutService;
            RegisterService = registerService;
        }

        protected void ScanShoppingCart()
        {
            DialogService.ShowDialog("BarcodeScanDialog", null, (scanResult) =>
            {
                var result = scanResult.Parameters.GetValue<string>("ScanData");
                if (!string.IsNullOrEmpty(result))
                {
                    var inputValue = "";
                    try
                    {
                        var definition = new { S = "", C = "" };
                        var qrFromKiosk = JsonConvert.DeserializeAnonymousType(result, definition);
                        inputValue = qrFromKiosk.S;
                    }
                    catch
                    {
                        inputValue = result;
                    }
                    ScanShoppingCartCallback(inputValue);
                }
            });
        }

        protected async Task ValidateShoppingCartAsync(string shoppingCart)
        {
            try
            {
                IsBusy = true;

                var validateResult = await SelfCheckoutService.ValidateShoppingCartAsync(SaleEngineService.LoginData.UserInfo.MachineEnv.MachineIp, shoppingCart);
                if (!validateResult.IsCompleted)
                {
                    DialogService.ShowAlert(AppResources.Opps, validateResult.DefaultMessage, AppResources.Close);
                    return;
                }

                SelfCheckoutService.CurrentShoppingCart = shoppingCart;

                var payload = new
                {
                    shoppingCard = shoppingCart,
                    SubBranch = SelfCheckoutService.AppConfig.SubBranch,
                    isTour = false,
                    isGenPdfPromotion = false,
                    isGenImgShoppingCard = false
                };

                var customersData = await RegisterService.GetCustomerAsync(payload);

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
                        {
                            await StartSessionAsync(shoppingCart);
                        }
                    });
                }
                else
                {
                    await StartSessionAsync(shoppingCart);
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

        async Task StartSessionAsync(string shoppingCart)
        {
            var userInfo = SaleEngineService.LoginData?.UserInfo;
            var startResult = await SelfCheckoutService.StartSessionAsync(userInfo.UserCode, userInfo.MachineEnv.MachineNo, shoppingCart);
            if (!startResult.IsCompleted)
            {
                DialogService.ShowAlert(AppResources.Opps, startResult.DefaultMessage, AppResources.Close);
                return;
            }

            await StartSessionCallbackAsync();
        }

        protected virtual Task StartSessionCallbackAsync()
        {
            return Task.FromResult(false);
        }

        protected virtual Task ScanShoppingCartCallback(string inputValue)
        {
            return Task.FromResult(false);
        }
    }
}
