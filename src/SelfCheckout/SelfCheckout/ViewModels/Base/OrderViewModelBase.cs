using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Print;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

        protected List<OrderInvoiceGroup> _allOrderInvoiceGroups;

        ObservableCollection<OrderInvoiceGroup> _orderInvoices;
        ObservableCollection<OrderDetail> _orderDetails;
        ObservableCollection<CustomerOrder> _customers;

        SessionData _sessionData;

        string _currencyCode;
        string _loginSession;

        int? _totalInvoice;
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

        public ObservableCollection<CustomerOrder> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public SessionData SessionData
        {
            get => _sessionData;
            set => SetProperty(ref _sessionData, value);
        }

        public string LoginSession
        {
            get => _loginSession;
            set => SetProperty(ref _loginSession, value);
        }

        public long BorrowSessionKey
        {
            get => SelfCheckoutService.BorrowSessionKey;
        }

        public int? TotalInvoice
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

        protected async Task<bool> LoadSessionDetailAsync(long sessionkey)
        {
            SessionData = await SelfCheckoutService.GetSessionDetialAsync(sessionkey);

            if (SessionData.SessionStatus.SessionCode == "END")
            {
                return true;
            }
            return false;
        }

        protected async Task SaveSessionAsync(string sessionKey)
        {
            //var appSetting = SelfCheckoutService.AppConfig;
            //var machineNo = SaleEngineService.LoginData.UserInfo.MachineEnv.MachineNo;
            //await SelfCheckoutService.EndSessionAsync(Convert.ToInt64(sessionKey), appSetting.UserName, machineNo);

            foreach (var orderInvoice in OrderInvoices)
            {
                var invoices = await SaleEngineService.PrintTaxInvoice(new
                {
                    OrderNo = orderInvoice.OrderNo,
                    ClaimcheckNo = "",
                    SessionKey = LoginSession
                });

                var invoiceImgUrl = invoices.FirstOrDefault()?.Data.Original.FirstOrDefault().Value;
                ///await DependencyService.Get<IPrintService>().PrintBitmapFromUrl(invoiceImgUrl);
            }
        }

        protected void Clear()
        {
            OrderInvoices?.Clear();
            OrderDetails?.Clear();
            TotalInvoice = null;
            TotalQty = null;
        }

        protected async Task<CustomerData> GetCustomerSessionAsync(string shoppingCard)
        {
            var customers = await RegisterService.GetCustomerAsync(shoppingCard);
            return customers.FirstOrDefault();
        }

        protected async Task LoadCustomerSession()
        {
            var sessionsGroup = SessionData.SesionDetail.GroupBy(s => s.ShoppingCard, (k, g) => new { ShoppingCard = k, SessionDetails = g.ToList() }).ToList();
            var customers = new List<CustomerOrder>();
            customers.Add(new CustomerOrder
            {
                CustomerName = AppResources.All
            });
            foreach (var sessionGroup in sessionsGroup)
            {
                try
                {
                    var shoppingCard = sessionGroup.ShoppingCard;
                    var result = await RegisterService.GetCustomerAsync(shoppingCard);
                    var customer = new CustomerOrder()
                    {
                        CustomerShoppingCard = shoppingCard,
                        CustomerName = result.FirstOrDefault()?.Person.EnglishName
                    };
                    customer.SessionDetails.AddRange(sessionGroup.SessionDetails);
                    customers.Add(customer);
                }
                catch { }
            }
            Customers = customers.ToObservableCollection();
        }

        protected async Task LoadOrderListAsync(object filter = null)
        {
            var appConfig = SelfCheckoutService.AppConfig;
            var loginResult = await SaleEngineService.LoginAsync(appConfig.UserName, appConfig.Password);
            LoginSession = loginResult.SessionKey;

            var ordersData = new List<OrderData>();

            var customers = Customers.Where(c => !string.IsNullOrEmpty(c.CustomerShoppingCard)).ToList();
            foreach (var customer in customers)
            {
                var payload = new
                {
                    SessionKey = loginResult.SessionKey,
                    Attributes = new object[]
                {
                    new
                    {
                        GROUP = "tran_no",
                        CODE = "shopping_card",
                        valueOfString = customer.CustomerShoppingCard
                    }
                },
                    filter = filter,
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

                var result = await SaleEngineService.GetOrderListAsync(payload);
                try
                {
                    result.ForEach(o => o.CustomerDetail.CustomerName = customer.CustomerName);
                }
                catch { }
                ordersData.AddRange(result);
            }

            _allOrderInvoiceGroups = new List<OrderInvoiceGroup>();
            foreach (var order in ordersData)
            {
                var orderInvoice = order.OrderInvoices.FirstOrDefault();
                var orderDetails = new List<OrderDetail>(orderInvoice.OrderDetails);

                var customerAttr = order.CustomerDetail?.CustomerAttributes;
                var orderInvoiceGroup = new OrderInvoiceGroup(orderDetails)
                {
                    InvoiceNo = order.OrderInvoices.FirstOrDefault()?.InvoiceNo.ToString(),
                    OrderNo = Convert.ToInt64(order.HeaderAttributes.Where(attr => attr.Code == "order_no").FirstOrDefault()?.ValueOfDecimal),
                    InvoiceDateTime = orderInvoice.Cashier.MachineDateTime, // TODO: concern
                    CustomerName = order.CustomerDetail?.CustomerName,
                    PassportNo = customerAttr?.Where(c => c.Code == "passport_no").FirstOrDefault()?.ValueOfString,
                    ShoppingCardNo = order.HeaderAttributes.Where(attr => attr.Code == "shopping_card").FirstOrDefault()?.ValueOfString,
                    CurrencyCode = orderInvoice.BillingAmount.NetAmount.CurrCode.Code,
                    PaymentType = orderInvoice.OrderPayments.FirstOrDefault()?.PaymentType,
                    TotalQty = orderInvoice.BillingQuantity.Quantity,
                    SubTotal = orderInvoice.BillingAmount.TotalAmount.CurrAmt,
                    TotalNet = orderInvoice.BillingAmount.NetAmount.CurrAmt,
                    TotalDiscount = orderInvoice.BillingAmount.DiscountAmount.CurrAmt
                };

                _allOrderInvoiceGroups.Add(orderInvoiceGroup);
            }

            var sessionDetails = new List<SesionDetail>();
            foreach (var customer in Customers)
            {
                sessionDetails.AddRange(customer.SessionDetails);
            }
            var ordersNo = sessionDetails.Select(s => s.OrderNo).ToList();
            _allOrderInvoiceGroups = _allOrderInvoiceGroups.Where(o => ordersNo.Contains(o.OrderNo)).ToList();
            OrderInvoices = _allOrderInvoiceGroups.ToObservableCollection();

            OrderDetails = new ObservableCollection<OrderDetail>();
            foreach (var order in _allOrderInvoiceGroups)
            {
                order.ForEach((orderDetail) => OrderDetails.Add(orderDetail));
            }
            CalculateSummary();
        }

        protected void CalculateSummary()
        {
            TotalInvoice = OrderInvoices.Count();
            CurrencyCode = OrderInvoices.FirstOrDefault()?.CurrencyCode;
            TotalQty = OrderInvoices.Sum(o => o.TotalQty);
            SubTotal = OrderInvoices.Sum(o => o.SubTotal);
            TotalDiscount = OrderInvoices.Sum(o => o.TotalDiscount);
            TotalNetAmount = OrderInvoices.Sum(o => o.TotalNet);
        }
    }
}
