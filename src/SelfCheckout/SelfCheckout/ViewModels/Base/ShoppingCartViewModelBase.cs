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

        protected void ScanShoppingCard()
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
                    ScanShoppingCardCallback(inputValue);
                }
            });
        }

        protected async Task ValidateShoppingCardAsync(string shoppingCard)
        {
            try
            {
                IsBusy = true;

                await SelfCheckoutService.ValidateShoppingCardAsync(SaleEngineService.LoginData.UserInfo.MachineEnv.MachineIp, shoppingCard);

                SelfCheckoutService.CurrentShoppingCard = shoppingCard;

                var customersData = await RegisterService.GetCustomerAsync(shoppingCard);

                var customerData = customersData.FirstOrDefault();
                if (customerData?.Person != null)
                {
                    if (!customerData.Person.IsActivate)
                    {
                        await DialogService.ShowAlert(AppResources.Opps, AppResources.ShoppingCardNotActivate, AppResources.Close);
                    }
                    var parameters = new DialogParameters()
                    {
                        {"Person", customerData.Person }
                    };
                    DialogService.ShowDialog("CustomerCardConfirmDialog", parameters, async (dialogResult) =>
                    {
                        var isConfirm = dialogResult.Parameters.GetValue<bool>("IsConfirm");
                        if (isConfirm)
                        {
                            await ValidateShoppingCardCallback(shoppingCard);
                        }
                    });
                }
                else
                {
                    await ValidateShoppingCardCallback(shoppingCard);
                }
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

        protected virtual Task ValidateShoppingCardCallback(string shoppingCard)
        {
            return Task.FromResult(false);
        }

        protected virtual Task ScanShoppingCardCallback(string shoppingCard)
        {
            return Task.FromResult(false);
        }
    }
}
