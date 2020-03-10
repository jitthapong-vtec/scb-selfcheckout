using Prism.Commands;
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        ISaleEngineService _saleEngineService;
        ISelfCheckoutService _selfCheckoutService;
        IRegisterService _registerService;

        ObservableCollection<SimpleSelectedItem> _tabs;
        ObservableCollection<OrderInvoiceGroup> _orderInvoices;

        public OrderViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService)
        {
            _saleEngineService = saleEngineService;
            _selfCheckoutService = selfCheckoutService;
            _registerService = registerService;

            Tabs = new ObservableCollection<SimpleSelectedItem>()
            {
                new SimpleSelectedItem()
                {
                    Text1 = "",
                    Selected = true,
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = "",
                    Arg1 = 2
                }
            };
        }

        public ICommand CustomerFilterTappedCommand => new DelegateCommand(() =>
        {

        });

        public ObservableCollection<SimpleSelectedItem> Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        public CustomerData CustomerData
        {
            get => _registerService.CustomerData;
        }

        public LoginData LoginData
        {
            get => _saleEngineService.LoginData;
        }

        public ObservableCollection<OrderInvoiceGroup> OrderInvoices
        {
            get => _orderInvoices;
            set => SetProperty(ref _orderInvoices, value);
        }

        public override async Task OnTabSelected(TabItem item)
        {
            await LoadSessionDetailAsync();
            //await LoadOrderListAsync();
        }

        async Task LoadSessionDetailAsync()
        {
            try
            {
                var result = await _selfCheckoutService.GetSessionDetialAsync(_selfCheckoutService.CurrentSessionKey);
            }
            catch(Exception ex)
            {

            }
            finally
            {

            }
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
                            valueOfString = "3600000711400"
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
                var orderData = _saleEngineService.OrderData;

                var t1 = Tabs.Where(t => (int)t.Arg1 == 1).FirstOrDefault();
                var t2 = Tabs.Where(t => (int)t.Arg1 == 2).FirstOrDefault();

                t1.Text1 = $"{orderData.TotalInvoice}";
                t2.Text1 = $"{orderData.BillingQty} {orderData.BillingUnit}";

                var orderInvoices = new List<OrderInvoiceGroup>();
                foreach (var orderInvoice in orderData.OrderInvoices)
                {
                    orderInvoices.Add(new OrderInvoiceGroup()
                    {
                        ViewType = 0
                    });
                }
                OrderInvoices = orderInvoices.ToObservableCollection();

                MessagingCenter.Send<ViewModelBase>(this, "OrderRefresh");
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
        }
    }
}
