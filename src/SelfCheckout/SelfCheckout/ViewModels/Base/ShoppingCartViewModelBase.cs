using Newtonsoft.Json;
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

        public ShoppingCartViewModelBase(IDialogService dialogService, ISaleEngineService saleEngineService,
            ISelfCheckoutService selfCheckoutService, IRegisterService registerService) : base(dialogService)
        {
            SaleEngineService = saleEngineService;
            SelfCheckoutService = selfCheckoutService;
            RegisterService = registerService;
        }

        protected string DecodeShoppingCardData(string scanData)
        {
            var data = "";
            if (!string.IsNullOrEmpty(scanData))
            {
                try
                {
                    var definition = new { S = "", C = "" };
                    var qrFromKiosk = JsonConvert.DeserializeAnonymousType(scanData, definition);
                    data = qrFromKiosk.S;
                }
                catch
                {
                    data = scanData;
                }
            }
            return data;
        }

        protected async Task ValidateShoppingCardAsync(string shoppingCard)
        {
            try
            {
                IsBusy = true;

                await SelfCheckoutService.ValidateShoppingCardAsync(GlobalSettings.Instance.MachineIp, shoppingCard);

                var customersData = await RegisterService.GetCustomerAsync(shoppingCard);

                var customerData = customersData.FirstOrDefault();
                if (customerData?.Person != null)
                {
                    if (!customerData.Person.IsActivate)
                    {
                        await DialogService.ShowAlert(AppResources.Opps, AppResources.ShoppingCardNotActivate, AppResources.Close);
                        return;
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
                            shoppingCard = customerData.Person?.ListIdentity?.Where(i => i.IdentityType == "SHOPCARD").FirstOrDefault()?.IdentityValue;
                            if (string.IsNullOrEmpty(shoppingCard))
                            {
                                await DialogService.ShowAlert(AppResources.Opps, "Can't get SHOPCARD", AppResources.Close);
                                return;
                            }
                            SelfCheckoutService.CurrentShoppingCard = shoppingCard;
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
