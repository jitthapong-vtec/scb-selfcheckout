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
    public class OrderViewModel : OrderViewModelBase
    {
        ObservableCollection<SimpleSelectedItem> _tabs;

        CustomerOrder _selectedCustomer;
        CustomerData _customerData;

        bool _isShowGroup;
        bool _summaryShowing;
        bool _filterCustomerShowing;

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
                IsBusy = true;
                var isAlreadyEnd = await LoadSessionDetailAsync(SelfCheckoutService.BorrowSessionKey);
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

                    await LoadOrderListAsync();
                }

                RefreshTab();
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
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
                OrderInvoices = _allOrderInvoiceGroups.Where(o => ordersNo.Contains(o.OrderNo)).ToList().ToObservableCollection();

            OrderDetails = new ObservableCollection<OrderDetail>();
            foreach (var order in OrderInvoices)
            {
                order.ForEach((orderDetail) => OrderDetails.Add(orderDetail));
            }

            CalculateSummary();
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
