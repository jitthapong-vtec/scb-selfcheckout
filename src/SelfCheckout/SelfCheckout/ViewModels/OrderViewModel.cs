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
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        ISaleEngineService _saleEngineService;
        ISelfCheckoutService _selfCheckoutService;
        IRegisterService _registerService;

        ObservableCollection<OrderDetail> _orderDetails;
        
        public CustomerData CustomerData
        {
            get => _registerService.CustomerData;
        }

        public LoginData LoginData
        {
            get => _saleEngineService.LoginData;
        }

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set => SetProperty(ref _orderDetails, value);
        }

        public OrderViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService)
        {
            _saleEngineService = saleEngineService;
            _selfCheckoutService = selfCheckoutService;
            _registerService = registerService;
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
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                    Attributes = new object[]
                    {
                        new
                        {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = _selfCheckoutService.CurrentShoppingCart
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

                var result = await _saleEngineService.GetOrderListAsync(payload);
                if (result.IsCompleted)
                {
                    var orderDetails = new List<OrderDetail>();
                    foreach (var orderInvoice in _saleEngineService.OrderData.OrderInvoices)
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
