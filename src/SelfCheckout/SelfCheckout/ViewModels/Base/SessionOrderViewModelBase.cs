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
        public SessionOrderViewModelBase(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        protected void ShowSessionOrder(DeviceStatus sess)
        {
            var parameters = new DialogParameters()
            {
                {"SessionKey", sess.SessionKey },
                {"ShoppingCard", sess.ShoppingCard }
            };
            try
            {
                DialogService.ShowDialog("SessionOrderDialog", parameters, async (result) =>
                {
                    if (result != null && result.Parameters.GetValue<bool>("IsConfirmed"))
                    {
                        var cf = await DialogService.ConfirmAsync(AppResources.SaveSession, AppResources.SaveSessionConfirm, AppResources.Yes, AppResources.No);
                        if (!cf)
                            return;

                        OrderInvoices = result.Parameters.GetValue<ObservableCollection<OrderInvoiceGroup>>("OrderInvoices");
                        try
                        {
                            IsBusy = true;
                            await SaveSessionAsync(sess.SessionKey.ToString());
                            await SessionCloseCallback();
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
                });
            }
            catch { }
        }

        protected virtual Task SessionCloseCallback()
        {
            return Task.FromResult(false);
        }
    }
}
