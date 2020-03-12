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
using System.Globalization;
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

        string _currencyCode;
        int _totalInvoice;
        double? _totalQty;
        double? _subTotal;
        double? _totalDiscount;
        double? _totalNetAmount;

        bool _summaryShowing;

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

        public ICommand ShowSummaryCommand => new Command(() =>
        {
            SummaryShowing = !SummaryShowing;
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
            get => _selfCheckoutService.CurrentShoppingCard;
        }

        public ObservableCollection<OrderInvoiceGroup> OrderInvoices
        {
            get => _orderInvoices;
            set => SetProperty(ref _orderInvoices, value);
        }

        public bool SummaryShowing
        {
            get => _summaryShowing;
            set => SetProperty(ref _summaryShowing, value);
        }

        public override async Task OnTabSelected(TabItem item)
        {
            //await LoadSessionDetailAsync();
            await LoadOrderListAsync();
        }

        async Task LoadSessionDetailAsync()
        {
            try
            {
                var result = await _selfCheckoutService.GetSessionDetialAsync(_selfCheckoutService.CurrentSessionKey);
            }
            catch (Exception ex)
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
                            valueOfString = _selfCheckoutService.StartedShoppingCard
                        }
                    },
                    paging = new
                    {
                        pageNo = 1,
                        pageSize = 10
                    },
                    filter = new object[]
                    {
                        new
                        {
                            sign = "string",
                            element = "order_data",
                            option = "string",
                            type = "string",
                            low = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            height = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        }
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

                var result = await _saleEngineService.GetOrderListAsync(payload);
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
                CurrencyCode = orderInvoiceGroups.FirstOrDefault().CurrencyCode;
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
