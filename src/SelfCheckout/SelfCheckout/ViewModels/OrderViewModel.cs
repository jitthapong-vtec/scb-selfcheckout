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
        CustomerOrder _selectedCustomer;
        CustomerData _customerData;

        bool _isShowGroup;
        bool _summaryShowing;
        bool _filterCustomerShowing;

        public OrderViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService)
            : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
            SelectedCustomer = new CustomerOrder
            {
                CustomerName = AppResources.All
            };

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

            MessagingCenter.Subscribe<MainViewModel>(this, "CurrencyChanged", async (s) =>
            {
                try
                {
                    await RefreshOrderAsync();
                }
                catch { }
            });

            IsShowGroup = true;
        }

        private async Task RefreshOrderAsync()
        {
            try
            {
                IsBusy = true;
                var sessionData = await GetSessionDetailAsync(SelfCheckoutService.BorrowSessionKey.ToString());
                await LoadCustomerSession(sessionData.SesionDetail);
                await LoadOrderListAsync(sessionData.ShoppingCard);
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ICommand TabSelectedCommand => new DelegateCommand<SimpleSelectedItem>((item) =>
        {
            var seletedItem = Tabs.Where(t => t.Selected).FirstOrDefault();
            seletedItem.Selected = false;

            if ((int)item.Arg1 == 1)
                IsShowGroup = true;
            else
                IsShowGroup = false;

            item.Selected = true;
        });

        public ICommand CustomerSelectionCommand => new DelegateCommand<CustomerOrder>((customer) =>
        {
            SelectedCustomer = customer;

            FilterOrder(customer.OrderNo);
            FilterCustomerShowing = false;
        });

        public ICommand ShowCustomerSelectionCommand => new DelegateCommand(() =>
        {
            FilterCustomerShowing = !FilterCustomerShowing;
        });

        public ICommand ShowSummaryCommand => new DelegateCommand(() =>
        {
            SummaryShowing = !SummaryShowing;
        });

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

        public bool IsShowGroup
        {
            get => _isShowGroup;
            set => SetProperty(ref _isShowGroup, value);
        }

        public bool SummaryShowing
        {
            get => _summaryShowing;
            set => SetProperty(ref _summaryShowing, value);
        }

        public override async Task OnTabSelected(TabItem item)
        {
            CustomerData = await GetCustomerSessionAsync(CurrentShoppingCard);
            await RefreshOrderAsync();
        }
    }
}
