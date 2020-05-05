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
    public abstract class ShoppingCartViewModelBase : NavigatableViewModelBase
    {
        protected ISaleEngineService SaleEngineService { get; private set; }
        protected ISelfCheckoutService SelfCheckoutService { get; private set; }
        protected IRegisterService RegisterService { get; private set; }

        public ShoppingCartViewModelBase(INavigationService navigationService, ISaleEngineService saleEngineService,
            ISelfCheckoutService selfCheckoutService, IRegisterService registerService) : base(navigationService)
        {
            SaleEngineService = saleEngineService;
            SelfCheckoutService = selfCheckoutService;
            RegisterService = registerService;
        }

        protected async Task ValidateShoppingCardAsync(string shoppingCard)
        {
            if (string.IsNullOrEmpty(shoppingCard))
                return;

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
                        await NavigationService.ShowAlertAsync(AppResources.Opps, AppResources.PleaseRegister, AppResources.Close);
                        await ValidateShoppingCardFailCallback();
                        return;
                    }
                    var parameters = new NavigationParameters()
                    {
                        {"Person", customerData.Person }
                    };
                    var dialogResult = await NavigationService.ShowDialogAsync<INavigationParameters>("CustomerCardConfirmDialog", parameters);

                    var isConfirm = dialogResult.GetValue<bool>("IsConfirm");
                    if (isConfirm)
                    {
                        shoppingCard = customerData.Person?.ListIdentity?.Where(i => i.IdentityType == "SHOPCARD").FirstOrDefault()?.IdentityValue;
                        if (string.IsNullOrEmpty(shoppingCard))
                        {
                            await NavigationService.ShowAlertAsync(AppResources.Opps, "Can't get SHOPCARD", AppResources.Close);
                            return;
                        }
                        SelfCheckoutService.CurrentShoppingCard = shoppingCard;
                        await ValidateShoppingCardCallback(shoppingCard);
                    }
                    else
                    {
                        await ValidateShoppingCardFailCallback();
                    }
                }
                else
                {
                    await ValidateShoppingCardCallback(shoppingCard);
                }
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
                await ValidateShoppingCardFailCallback();
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual Task ValidateShoppingCardFailCallback()
        {
            return Task.FromResult(false);
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
