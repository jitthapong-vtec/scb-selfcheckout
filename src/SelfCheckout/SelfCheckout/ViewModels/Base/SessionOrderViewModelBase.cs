using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class SessionOrderViewModelBase : OrderViewModelBase
    {
        public SessionOrderViewModelBase(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService) :
            base(navigationService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        protected async Task ShowSessionOrder(DeviceStatus sess)
        {
            var parameters = new NavigationParameters()
            {
                {"SessionKey", sess.SessionKey },
                {"ShoppingCard", sess.ShoppingCard },
                {"SessionDate", sess.SessionDt }
            };
            try
            {
                var result = await NavigationService.ShowDialogAsync<INavigationParameters>("SessionOrderDialog", parameters);

                if (result != null && result.GetValue<bool>("IsConfirmed"))
                {
                    var cf = await NavigationService.ConfirmAsync(AppResources.SaveSession, AppResources.SaveSessionConfirm, AppResources.Yes, AppResources.No);
                    if (!cf)
                        return;

                    OrderInvoices = result.GetValue<ObservableCollection<OrderInvoiceGroup>>("OrderInvoices");
                    try
                    {
                        IsBusy = true;
                        await SaveSessionAsync(sess.SessionKey.ToString());
                        await SessionCloseCallback();
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
            catch { }
        }

        protected virtual Task SessionCloseCallback()
        {
            return Task.FromResult(false);
        }
    }
}
