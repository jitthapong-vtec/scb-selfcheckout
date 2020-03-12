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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class OrderViewModelBase : ViewModelBase
    {
        protected ISaleEngineService SaleEngineService { get; private set; }
        protected ISelfCheckoutService SelfCheckoutService { get; private set; }
        protected IRegisterService RegisterService { get; private set; }

        ObservableCollection<SimpleSelectedItem> _tabs;
        ObservableCollection<OrderInvoiceGroup> _orderInvoices;
        SessionData _sessionData;

        string _currencyCode;
        int _totalInvoice;
        double? _totalQty;
        double? _subTotal;
        double? _totalDiscount;
        double? _totalNetAmount;

        public OrderViewModelBase(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService)
        {
            SaleEngineService = saleEngineService;
            SelfCheckoutService = selfCheckoutService;
            RegisterService = registerService;
        }

        public ObservableCollection<SimpleSelectedItem> Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        public CustomerData CustomerData
        {
            get => RegisterService.CustomerData;
        }

        public LoginData LoginData
        {
            get => SaleEngineService.LoginData;
        }

        public SessionData SessionData
        {
            get => _sessionData;
            set => SetProperty(ref _sessionData, value);
        }

        public int TotalInvoice
        {
            get => _totalInvoice;
            set => SetProperty(ref _totalInvoice, value);
        }

        public double? TotalQty
        {
            get => _totalQty;
            set => SetProperty(ref _totalQty, value);
        }

        public double? SubTotal
        {
            get => _subTotal;
            set => SetProperty(ref _subTotal, value);
        }

        public double? TotalDiscount
        {
            get => _totalDiscount;
            set => SetProperty(ref _totalDiscount, value);
        }

        public string CurrencyCode
        {
            get => _currencyCode;
            set => SetProperty(ref _currencyCode, value);
        }

        public double? TotalNetAmount
        {
            get => _totalNetAmount;
            set => SetProperty(ref _totalNetAmount, value);
        }

        public string CurrentShoppingCard
        {
            get => SelfCheckoutService.CurrentShoppingCard;
        }

        public ObservableCollection<OrderInvoiceGroup> OrderInvoices
        {
            get => _orderInvoices;
            set => SetProperty(ref _orderInvoices, value);
        }

        protected async Task<bool> GetSessionDetailAsync(string sessionkey)
        {
            try
            {
                IsBusy = true;
                var result = await SelfCheckoutService.GetSessionDetialAsync(sessionkey);
                SessionData = result.Data;
                return true;
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
            return false;
        }

        protected async Task GetOrderListAsync(string loginSessionKey, string shoppingCard)
        {
            try
            {
                var payload = new
                {
                    SessionKey = loginSessionKey,
                    Attributes = new object[]
                    {
                        new
                        {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = shoppingCard
                        }
                    },
                    paging = new
                    {
                        pageNo = 1,
                        pageSize = 100
                    },
                    //filter = new object[]
                    //{
                    //    new
                    //    {
                    //        sign = "string",
                    //        element = "order_data",
                    //        option = "string",
                    //        type = "string",
                    //        low = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    //        height = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    //    }
                    //},
                    sorting = new object[]
                    {
                        new
                        {
                            sortBy = "headerkey",
                            orderBy = "desc"
                        }
                    }
                };

                var result = await SaleEngineService.GetOrderListAsync(payload);
                var ordersData = result.Data;

                var orderInvoiceGroups = new List<OrderInvoiceGroup>();
                foreach (var order in ordersData)
                {
                    var orderInvoice = order.OrderInvoices.FirstOrDefault();
                    var orderDetails = new List<OrderDetail>(orderInvoice.OrderDetails);

                    var customerAttr = order.CustomerDetail?.CustomerAttributes;
                    var orderInvoiceGroup = new OrderInvoiceGroup(orderDetails)
                    {
                        InvoiceNo = "12345",
                        InvoiceDateTime = orderInvoice.Cashier.MachineDateTime, // TODO: concern
                        CustomerName = order.CustomerDetail?.CustomerName,
                        PassportNo = customerAttr?.Where(c => c.Code == "passport_no").FirstOrDefault()?.ValueOfString,
                        ShoppingCardNo = customerAttr.Where(c => c.Code == "shopping_card").FirstOrDefault()?.ValueOfString,
                        CurrencyCode = orderInvoice.BillingAmount.NetAmount.CurrCode.Code,
                        PaymentType = orderInvoice.OrderPayments.FirstOrDefault()?.PaymentType,
                        TotalQty = orderInvoice.BillingQuantity.Quantity,
                        SubTotal = orderInvoice.BillingAmount.TotalAmount.CurrAmt,
                        TotalNet = orderInvoice.BillingAmount.NetAmount.CurrAmt,
                        TotalDiscount = orderInvoice.BillingAmount.DiscountAmount.CurrAmt
                    };

                    orderInvoiceGroups.Add(orderInvoiceGroup);
                }
                OrderInvoices = orderInvoiceGroups.ToObservableCollection();

                TotalInvoice = ordersData.Count();
                CurrencyCode = orderInvoiceGroups.FirstOrDefault()?.CurrencyCode;
                TotalQty = orderInvoiceGroups.Sum(o => o.TotalQty);
                SubTotal = orderInvoiceGroups.Sum(o => o.SubTotal);
                TotalDiscount = orderInvoiceGroups.Sum(o => o.TotalDiscount);
                TotalNetAmount = orderInvoiceGroups.Sum(o => o.TotalNet);

                var t1 = Tabs.Where(t => (int)t.Arg1 == 1).FirstOrDefault();
                var t2 = Tabs.Where(t => (int)t.Arg1 == 2).FirstOrDefault();

                t1.Text1 = $"{TotalInvoice} {AppResources.Invoices}";
                t2.Text1 = $"{TotalQty} {AppResources.Units}";

                MessagingCenter.Send<ViewModelBase>(this, "OrderRefresh");
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
        }
    }
}
