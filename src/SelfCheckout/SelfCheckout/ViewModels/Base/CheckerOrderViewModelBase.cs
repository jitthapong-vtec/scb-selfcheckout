using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public class CheckerOrderViewModelBase : OrderViewModelBase
    {
        string _sessionKey;
        CustomerData _customerData;

        public CheckerOrderViewModelBase(ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(selfCheckoutService, saleEngineService, registerService)
        {
        }

        public string SessionKey
        {
            get => _sessionKey;
            set => SetProperty(ref _sessionKey, value);
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public ICommand SaveSessionCommand => new Command(async () =>
        {
            if (SessionData == null)
                return;

            var result = await _dialogService.ConfirmAsync(AppResources.SaveSession, AppResources.SaveSessionConfirm, AppResources.Yes, AppResources.No);
            if (!result)
                return;

            try
            {
                IsBusy = true;
                await SaveSessionAsync(SessionKey);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        });

        protected async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                var isAlreadyEnd = await LoadSessionDetailAsync(Convert.ToInt64(SessionKey));
                if (isAlreadyEnd)
                {
                    Clear();
                    await _dialogService.ShowAlert(AppResources.Alert, AppResources.SessionAlreadyFinish, AppResources.Close);
                    return;
                }

                await LoadCustomerSession();

                CustomerData = await GetCustomerSessionAsync(SessionData.ShoppingCard);

                await LoadOrderListAsync();
                GroupingOrder();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected void GroupingOrder()
        {
            var groups = OrderDetails.GroupBy(o => o.ItemDetail.Item.Code, (k, g) =>
                                new
                                {
                                    Order = g.FirstOrDefault(),
                                    TotalQty = g.Sum(o => o.BillingQuantity.Quantity),
                                    TotalAmount = g.Sum(o => o.BillingAmount.TotalAmount.CurrAmt),
                                    TotalDiscount = g.Sum(o => o.BillingAmount.DiscountAmount.CurrAmt),
                                    TotalNet = g.Sum(o => o.BillingAmount.NetAmount.CurrAmt),
                                    OrderDetails = g.ToList()
                                }).ToList();

            var temp = new List<OrderDetail>();
            foreach (var group in groups)
            {
                group.Order.BillingQuantity.Quantity = group.TotalQty;
                group.Order.BillingAmount.TotalAmount.CurrAmt = group.TotalAmount;
                group.Order.BillingAmount.DiscountAmount.CurrAmt = group.TotalDiscount;
                group.Order.BillingAmount.NetAmount.CurrAmt = group.TotalNet;
                temp.Add(group.Order);
            }
            OrderDetails = temp.ToObservableCollection();
        }
    }
}
