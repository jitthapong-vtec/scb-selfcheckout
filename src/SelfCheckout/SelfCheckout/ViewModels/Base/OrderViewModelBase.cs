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

        List<OrderInvoiceGroup> _allOrderInvoiceGroups;

        ObservableCollection<SimpleSelectedItem> _tabs;
        ObservableCollection<OrderInvoiceGroup> _orderInvoices;
        ObservableCollection<OrderDetail> _orderDetails;
        ObservableCollection<CustomerOrder> _customers;

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

        public ObservableCollection<CustomerOrder> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public int BorrowSessionKey
        {
            get => SelfCheckoutService.BorrowSessionKey;
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

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set => SetProperty(ref _orderDetails, value);
        }

        protected async Task<SessionData> GetSessionDetailAsync(string sessionkey)
        {
            return await SelfCheckoutService.GetSessionDetialAsync(sessionkey);
        }

        protected async Task<CustomerData> GetCustomerSessionAsync(string shoppingCard)
        {
            var customers = await RegisterService.GetCustomerAsync(shoppingCard);
            return customers.FirstOrDefault();
        }

        protected async Task LoadCustomerSession(List<SesionDetail> sessionDetails)
        {
            var customers = new List<CustomerOrder>();
            customers.Add(new CustomerOrder
            {
                CustomerName = AppResources.All
            });
            foreach (var sessionDetail in sessionDetails)
            {
                try
                {
                    var shoppingCard = sessionDetail.ShoppingCard;
                    var result = await RegisterService.GetCustomerAsync(shoppingCard);
                    customers.Add(new CustomerOrder()
                    {
                        OrderNo = $"{sessionDetail.OrderNo}",
                        SessionKey = $"{sessionDetail.SessionDetailKey}",
                        CustomerName = result.FirstOrDefault()?.Person.EnglishName
                    });
                }
                catch { }
            }
            Customers = customers.ToObservableCollection();
        }

        protected async Task LoadOrderListAsync(string shoppingCard)
        {
            var appConfig = SelfCheckoutService.AppConfig;
            var loginResult = await SaleEngineService.LoginAsync(appConfig.UserName, appConfig.Password);

            var payload = new
            {
                SessionKey = loginResult.SessionKey,
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
                sorting = new object[]
                {
                    new
                    {
                        sortBy = "headerkey",
                        orderBy = "desc"
                    }
                }
            };

            var ordersData = await SaleEngineService.GetOrderListAsync(payload);

            _allOrderInvoiceGroups = new List<OrderInvoiceGroup>();
            var allOrderDetails = new List<OrderDetail>();
            foreach (var order in ordersData)
            {
                var orderInvoice = order.OrderInvoices.FirstOrDefault();
                var orderDetails = new List<OrderDetail>(orderInvoice.OrderDetails);

                allOrderDetails.AddRange(orderDetails);

                var customerAttr = order.CustomerDetail?.CustomerAttributes;
                var orderInvoiceGroup = new OrderInvoiceGroup(orderDetails)
                {
                    InvoiceNo = "12345",
                    OrderNo = $"{order.HeaderAttributes.Where(attr => attr.Code == "order_no").Select(o => o.ValueOfDecimal).FirstOrDefault()}",
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

                _allOrderInvoiceGroups.Add(orderInvoiceGroup);
            }
            OrderInvoices = _allOrderInvoiceGroups.ToObservableCollection();
            OrderDetails = allOrderDetails.ToObservableCollection();

            CalculateSummary();

            if (Device.Idiom == TargetIdiom.Phone)
            {
                var t2 = Tabs.Where(t => (int)t.Arg1 == 2).FirstOrDefault();
                t2.Text1 = $"{TotalQty} {AppResources.Units}";
            }
        }

        protected void FilterOrder(string orderNo)
        {
            if (string.IsNullOrEmpty(orderNo))
                OrderInvoices = _allOrderInvoiceGroups.ToObservableCollection();
            else
                OrderInvoices = _allOrderInvoiceGroups.Where(o => o.OrderNo == orderNo).ToList().ToObservableCollection();
            CalculateSummary();
        }

        private void CalculateSummary()
        {
            if (Device.Idiom == TargetIdiom.Desktop)
                return;

            TotalInvoice = OrderInvoices.Count();
            CurrencyCode = OrderInvoices.FirstOrDefault()?.CurrencyCode;
            TotalQty = OrderInvoices.Sum(o => o.TotalQty);
            SubTotal = OrderInvoices.Sum(o => o.SubTotal);
            TotalDiscount = OrderInvoices.Sum(o => o.TotalDiscount);
            TotalNetAmount = OrderInvoices.Sum(o => o.TotalNet);

            var t1 = Tabs.Where(t => (int)t.Arg1 == 1).FirstOrDefault();
            t1.Text1 = $"{TotalInvoice} {AppResources.Invoices}";
        }
    }
}
