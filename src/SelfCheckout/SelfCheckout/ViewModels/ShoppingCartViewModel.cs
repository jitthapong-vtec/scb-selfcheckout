using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Exceptions;
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class ShoppingCartViewModel : ShoppingCartViewModelBase
    {
        object _lock = new object();

        public Func<Task> ReloadOrderDataAsync;
        public Action RefreshSummary;
        public Action<bool> ShoppingCardChanging;

        ObservableCollection<OrderDetail> _orderDetails;

        CustomerData _customerData;

        string _currentShoppingCard;

        bool _isSelectAllOrder;
        bool _isAnyOrderSelected;
        bool _isChangeShoppingCardShowing;

        string _labelAll;
        string _labelDelete;
        string _labelShoppingCartEmpty;
        string _labelChangeShoppingCard;

        public ShoppingCartViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService) :
            base(navigationService, saleEngineService, selfCheckoutService, registerService)
        {
            CustomerData = RegisterService.CustomerData;
            CurrentShoppingCard = SelfCheckoutService.CurrentShoppingCard;

            RefreshLanguage();
        }

        public void RefreshLanguage()
        {
            LabelAll = AppResources.All;
            LabelDelete = AppResources.Delete;
            LabelShoppingCartEmpty = AppResources.ShoppingCartEmpty;
            LabelChangeShoppingCard = AppResources.ChangeShoppingCard;
        }

        public ICommand ToggleChangeShoppingCardCommand => new DelegateCommand(() =>
        {
            IsChangeShoppingCardShowing = !IsChangeShoppingCardShowing;
        });

        public ICommand ChangeShoppingCardCommand => new DelegateCommand(async () =>
        {
            lock (_lock)
            {
                if (IsChangeShoppingCardShowing == false)
                    return;
                else
                    IsChangeShoppingCardShowing = false;
            }

            ShoppingCardChanging(true);

            var result = await NavigationService.ShowDialogAsync<INavigationParameters>("ShoppingCardInputDialog", null);
            var shoppingCard = result.GetValue<string>("ShoppingCard");
            if (!string.IsNullOrEmpty(shoppingCard))
                await ValidateShoppingCardAsync(shoppingCard);
            ShoppingCardChanging(false);
        });

        public ICommand SelectAllOrderCommand => new DelegateCommand(() =>
        {
            if (OrderDetails == null)
                return;
            if (!string.IsNullOrEmpty(SaleEngineService.CouponCode))
                return;

            IsSelectAllOrder = !IsSelectAllOrder;
            foreach (var order in OrderDetails)
            {
                order.IsSelected = IsSelectAllOrder;
            }
        });

        public ICommand OrderSelectedCommand => new DelegateCommand<OrderDetail>((order) =>
        {
            order.IsSelected = !order.IsSelected;

            try
            {
                IsAnyOrderSelected = OrderDetails.Where(o => o.IsSelected).Any();
                if (!IsAnyOrderSelected)
                    IsSelectAllOrder = false;
            }
            catch { }
        });

        public ICommand ChangeQtyCommand => new DelegateCommand<OrderDetail>(async (order) =>
        {
            await ChangeOrderQtyAsync(order);
        });

        private async Task ChangeOrderQtyAsync(OrderDetail order)
        {
            try
            {
                var qty = order.BillingQuantity.Quantity;

                var payload = new
                {
                    SessionKey = SaleEngineService.LoginData.SessionKey,
                    Rows = new string[] { order.Guid },
                    ActionItemValue = new
                    {
                        Action = "change_qty",
                        Value = $"{qty}"
                    }
                };
                await SetActionToOrder(payload);
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message);
                await ReloadOrderDataAsync();
            }
        }

        public ICommand DeleteOrderCommand => new DelegateCommand<OrderDetail>(async (order) =>
        {
            if (!string.IsNullOrEmpty(SaleEngineService.CouponCode))
                return;

            if (order != null)
            {
                var result = await NavigationService.ConfirmAsync(AppResources.ConfirmDeleteItem, order.ItemDetail.Item.Desc, AppResources.Yes, AppResources.No, true);

                if (result)
                {
                    await DeleteItemAsync(new OrderDetail[] { order });
                }
            }
            else
            {
                var selectedOrders = OrderDetails.Where(o => o.IsSelected).ToArray();
                if (selectedOrders.Any())
                {
                    var itemsNameToDelete = "";
                    foreach (var selectedOrder in selectedOrders)
                    {
                        itemsNameToDelete += $"{selectedOrder.ItemDetail.Item.Desc}\n";
                    }
                    var result = await NavigationService.ConfirmAsync(AppResources.ConfirmDeleteItem, itemsNameToDelete, AppResources.Yes, AppResources.No, true);

                    if (result)
                    {
                        await DeleteItemAsync(selectedOrders);
                    }
                }
            }
        });

        public ICommand ShowDetailCommand => new DelegateCommand<OrderDetail>(async (order) =>
        {
            await NavigationService.NavigateAsync("OrderDetailView", new NavigationParameters() { { "OrderDetail", order } });
        });

        public ICommand RefreshOrderCommand => new DelegateCommand(async () =>
        {
            IsRefreshing = true;
            await ReloadOrderDataAsync();
            IsRefreshing = false;
        });

        public string LabelAll
        {
            get => _labelAll;
            set => SetProperty(ref _labelAll, value);
        }

        public string LabelDelete
        {
            get => _labelDelete;
            set => SetProperty(ref _labelDelete, value);
        }

        public string LabelShoppingCartEmpty
        {
            get => _labelShoppingCartEmpty;
            set => SetProperty(ref _labelShoppingCartEmpty, value);
        }

        public string LabelChangeShoppingCard
        {
            get => _labelChangeShoppingCard;
            set => SetProperty(ref _labelChangeShoppingCard, value);
        }

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set => SetProperty(ref _orderDetails, value);
        }

        public string CurrentShoppingCard
        {
            get => _currentShoppingCard;
            set => SetProperty(ref _currentShoppingCard, value);
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public bool IsChangeShoppingCardShowing
        {
            get => _isChangeShoppingCardShowing;
            set => SetProperty(ref _isChangeShoppingCardShowing, value);
        }

        public bool IsSelectAllOrder
        {
            get => _isSelectAllOrder;
            set
            {
                SetProperty(ref _isSelectAllOrder, value, () =>
                {
                    IsAnyOrderSelected = value;
                });
            }
        }

        public bool IsAnyOrderSelected
        {
            get => _isAnyOrderSelected;
            set => SetProperty(ref _isAnyOrderSelected, value);
        }

        public bool IsFirstSelect { get; set; } = true;

        public override async Task OnTabSelected(TabItem item)
        {
            if (IsFirstSelect)
            {
                await ReloadOrderDataAsync();
            }
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            IsFirstSelect = true;
            return base.OnTabDeSelected(item);
        }

        protected override async Task ValidateShoppingCardCallback(string shoppingCard)
        {
            CustomerData = RegisterService.CustomerData;
            CurrentShoppingCard = SelfCheckoutService.CurrentShoppingCard;

            var appConfig = SelfCheckoutService.AppConfig;
            var loginResult = await SaleEngineService.LoginAsync(appConfig.UserName, appConfig.Password);
            SaleEngineService.LoginData = loginResult;

            await ReloadOrderDataAsync();
        }

        public Task RefreshOrderListAsync()
        {
            var orderDetails = SaleEngineService.OrderData?.OrderDetails;
            if (!string.IsNullOrEmpty(SaleEngineService.CouponCode))
            {
                orderDetails.ForEach(o => o.IsEditable = false);
            }
            OrderDetails = orderDetails?.ToObservableCollection();

            SetOrderImage(orderDetails);

            IsSelectAllOrder = false;
            IsAnyOrderSelected = false;
            return Task.FromResult(true);
        }

        async Task DeleteItemAsync(OrderDetail[] orders)
        {
            if (orders.Any())
            {
                var payload = new
                {
                    SessionKey = SaleEngineService.LoginData.SessionKey,
                    Rows = orders.Select(o => o.Guid).ToArray(),
                    ActionItemValue = new
                    {
                        Action = "delete",
                        Value = "1"
                    }
                };
                try
                {
                    await SetActionToOrder(payload);
                }
                catch (Exception ex)
                {
                    await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message);
                }
            }
        }

        async Task SetActionToOrder(object payload)
        {
            await SaleEngineService.ActionListItemToOrderAsync(payload);
            await RefreshOrderListAsync();
            RefreshSummary();
        }
    }
}
