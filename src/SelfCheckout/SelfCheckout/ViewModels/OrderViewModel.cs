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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class OrderViewModel : OrderViewModelBase
    {
        public Action<bool> ShowOrHideLoading;

        ObservableCollection<SimpleSelectedItem> _tabs;

        CustomerOrder _selectedCustomer;
        CustomerData _customerData;

        CustomerOrder _currentFilter;

        bool _isShowGroup;
        bool _summaryShowing;
        bool _filterCustomerShowing;

        string _labelPassportNo;
        string _labelNationality;
        string _labelType;
        string _labelFilter;

        public OrderViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService)
            : base(navigationService, selfCheckoutService, saleEngineService, registerService)
        {
            SelectedCustomer = new CustomerOrder
            {
                CustomerName = AppResources.All
            };

            Tabs = new ObservableCollection<SimpleSelectedItem>()
            {
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.Units,
                    Arg1 = 1,
                    Selected = true
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.Invoices,
                    Arg1 = 2
                }
            };

            RefreshLanguage();
        }

        public void RefreshLanguage()
        {
            LabelPassportNo = AppResources.PassportNo;
            LabelNationality = AppResources.Nationality;
            LabelType = AppResources.Type;
            LabelFilter = AppResources.Filter;
            if (!SelectedCustomer.SessionDetails.Any())
                SelectedCustomer.CustomerName = AppResources.All;
            RefreshTab();
        }

        public string LabelPassportNo
        {
            get => _labelPassportNo;
            set => SetProperty(ref _labelPassportNo, value);
        }

        public string LabelNationality
        {
            get => _labelNationality;
            set => SetProperty(ref _labelNationality, value);
        }

        public string LabelType
        {
            get => _labelType;
            set => SetProperty(ref _labelType, value);
        }

        public string LabelFilter
        {
            get => _labelFilter;
            set => SetProperty(ref _labelFilter, value);
        }

        public ObservableCollection<SimpleSelectedItem> Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        public async Task RefreshOrderAsync()
        {
            try
            {
                ShowOrHideLoading?.Invoke(true);
                var isAlreadyEnd = await LoadSessionDetailAsync(SelfCheckoutService.BorrowSessionKey.ToString());
                if (isAlreadyEnd)
                {
                    Clear();
                    await SaleEngineService.LogoutAsync();
                    await GoBackToRootAsync();
                }
                else
                {
                    await LoadCustomerSession();
                    CustomerData = await GetCustomerSessionAsync(SessionData.ShoppingCard);

                    var currencyCode = SaleEngineService.CurrencySelected.CurrCode;
                    var filter = new object[]
                    {
                        new
                        {
                            sign = "string",
                            element = "order_date",
                            option = "string",
                            type = "string",
                            low = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            high = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        }
                    };
                    await LoadOrderListAsync(filter: filter, currencyCode: currencyCode, groupingOrderDetail: true);
                    if (_currentFilter != null)
                        FilterOrder(_currentFilter);
                }

                RefreshTab();
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                ShowOrHideLoading?.Invoke(false);
            }
        }

        private void RefreshTab()
        {
            var t2 = Tabs.Where(t => (int)t.Arg1 == 1).FirstOrDefault();
            t2.Text1 = $"{TotalQty} {AppResources.Units}";

            var t1 = Tabs.Where(t => (int)t.Arg1 == 2).FirstOrDefault();
            t1.Text1 = $"{TotalInvoice} {AppResources.Invoices}";
        }

        void FilterOrder(CustomerOrder customer)
        {
            var ordersNo = customer.SessionDetails.Select(c => c.OrderNo).ToList();
            if (!customer.SessionDetails.Any())
                OrderInvoices = _allOrderInvoiceGroups.ToObservableCollection();
            else
                OrderInvoices = _allOrderInvoiceGroups.Where(o => 
                o.ShoppingCardNo.Equals(customer.CustomerShoppingCard, StringComparison.OrdinalIgnoreCase) 
                && ordersNo.Contains(o.OrderNo)).ToList().ToObservableCollection();

            var orderDetails = new List<OrderDetail>();
            foreach (var order in OrderInvoices)
            {
                order.ForEach((orderDetail) => orderDetails.Add(orderDetail));
            }
            var orderDetailGrouping = GroupingOrderDetail(orderDetails);
            OrderDetails = orderDetailGrouping.ToObservableCollection();

            _currentFilter = customer;

            RefreshSummary();
            RefreshTab();
        }

        public ICommand TabSelectedCommand => new DelegateCommand<SimpleSelectedItem>((item) =>
        {
            var seletedItem = Tabs.Where(t => t.Selected).FirstOrDefault();
            seletedItem.Selected = false;

            if ((int)item.Arg1 == 2)
                IsShowGroup = true;
            else
                IsShowGroup = false;

            item.Selected = true;
        });

        public ICommand CustomerSelectionCommand => new DelegateCommand<CustomerOrder>((customer) =>
        {
            SelectedCustomer = customer;

            FilterOrder(customer);
            FilterCustomerShowing = false;
        });

        public ICommand ShowDetailCommand => new DelegateCommand<OrderDetail>(async (order) =>
        {
            await NavigationService.NavigateAsync("OrderDetailView", new NavigationParameters() { { "OrderDetail", order } });
        });

        public ICommand ShowCustomerSelectionCommand => new DelegateCommand(() =>
        {
            FilterCustomerShowing = !FilterCustomerShowing;
        });

        public ICommand ShowSummaryCommand => new DelegateCommand(() =>
        {
            SummaryShowing = !SummaryShowing;
        });

        public bool IsShowGroup
        {
            get => _isShowGroup;
            set => SetProperty(ref _isShowGroup, value);
        }

        public CustomerOrder SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public bool FilterCustomerShowing
        {
            get => _filterCustomerShowing;
            set => SetProperty(ref _filterCustomerShowing, value);
        }

        public bool SummaryShowing
        {
            get => _summaryShowing;
            set => SetProperty(ref _summaryShowing, value);
        }

        public override async Task OnTabSelected(TabItem item)
        {
            await RefreshOrderAsync();
        }
    }
}
