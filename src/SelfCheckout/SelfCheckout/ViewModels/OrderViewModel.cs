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
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class OrderViewModel : OrderViewModelBase
    {
        public OrderViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public override async Task OnTabSelected(TabItem item)
        {
            await LoadOrderListAsync();
        }

        async Task LoadOrderListAsync()
        {
            try
            {
                var payload = new
                {
                    SessionKey = LoginData.SessionKey,
                    Attributes = new object[]
                    {
                        new
                        {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = CurrentShoppingCart
                        }
                    },
                    paging = new
                    {
                        pageNo = 1,
                        pageSize = 10
                    },
                    sorting = new object[]
                    {
                        new {
                            sortBy = "headerkey",
                            orderBy = "desc"
                        }
                    }
                };

                var result = await SaleEngineService.GetOrderListAsync(payload);
                if (result.IsCompleted)
                {
                    var orderDetails = new List<OrderDetail>();
                    foreach(var orderInvoice in SaleEngineService.OrderData.OrderInvoices)
                    {
                        orderDetails.AddRange(orderInvoice.OrderDetails);
                    }
                    OrderDetails = orderDetails.ToObservableCollection();
                }
                else
                {
                    DialogService.ShowAlert(AppResources.Opps, result.DefaultMessage);
                }
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
        }
    }
}
