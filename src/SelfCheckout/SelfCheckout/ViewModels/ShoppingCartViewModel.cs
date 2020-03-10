using Newtonsoft.Json;
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class ShoppingCartViewModel : ShoppingCartViewModelBase
    {
        ObservableCollection<OrderDetail> _orderDetails;
        CustomerData _customerData;

        string _currentShoppingCart;

        object[] items;

        bool _isSelectAllOrder;
        bool _isAnyOrderSelected;
        bool _isChangeShoppingCartShowing;

        public ShoppingCartViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, saleEngineService, selfCheckoutService, registerService)
        {
            items = new object[]
               {
                new
                {
                    SessionKey = SaleEngineService.LoginData.SessionKey,
                    ItemCode = "00008211470207673"
                },
                   //new
                   //{
                   //    SessionKey = LoginData.SessionKey,
                   //    ItemCode = "00008190415206226"
                   //}
               };

            CustomerData = RegisterService.CustomerData;
            CurrentShoppingCart = SelfCheckoutService.CurrentShoppingCart;
        }

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set => SetProperty(ref _orderDetails, value);
        }

        public string CurrentShoppingCart
        {
            get => _currentShoppingCart;
            set => SetProperty(ref _currentShoppingCart, value);
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public bool IsChangeShoppingCartShowing
        {
            get => _isChangeShoppingCartShowing;
            set => SetProperty(ref _isChangeShoppingCartShowing, value);
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

        public ICommand ToggleChangeShoppingCartCommand => new DelegateCommand(() =>
        {
            IsChangeShoppingCartShowing = !IsChangeShoppingCartShowing;
        });

        public ICommand ChangeShoppingCartCommand => new DelegateCommand(() =>
        {
            IsChangeShoppingCartShowing = false;
            DialogService.ShowDialog("ShoppingCartInputDialog", null, async (dialogResult) =>
            {
                var shoppingCart = dialogResult.Parameters.GetValue<string>("ShoppingCart");
                if (!string.IsNullOrEmpty(shoppingCart))
                    await ValidateShoppingCartAsync(shoppingCart);
            });
        });

        public ICommand SelectAllOrderCommand => new DelegateCommand(() =>
        {
            if (OrderDetails == null)
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
        });

        public ICommand DeleteOrderCommand => new DelegateCommand<OrderDetail>(async (order) =>
        {
            if (order != null)
            {
                await DeleteItemAsync(new OrderDetail[] { order });
            }
            else
            {
                var selectedOrders = OrderDetails.Where(o => o.IsSelected).ToArray();
                if (selectedOrders.Any())
                {
                    var result = await DialogService.ConfirmAsync(AppResources.Delete, AppResources.ConfirmDeleteItem, AppResources.Yes, AppResources.No);

                    if (result)
                    {
                        await DeleteItemAsync(selectedOrders);
                    }
                }
            }
        });

        public ICommand RefreshOrderCommand => new DelegateCommand(async () =>
        {
            IsRefreshing = true;
            await LoadOrderAsync();
            IsRefreshing = false;
        });

        public override async Task OnTabSelected(TabItem item)
        {
            if (IsFirstSelect)
            {
                await LoadOrderAsync();
                IsFirstSelect = false;
            }
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            IsFirstSelect = true;
            return base.OnTabDeSelected(item);
        }

        protected override async Task ValidateShoppingCartCallback(string shoppingCart)
        {
            try
            {
                IsBusy = true;
                var headerAttr = SaleEngineService.OrderData.HeaderAttributes.Where(o => o.Code == "order_no").FirstOrDefault();
                var orderNo = Convert.ToInt32(headerAttr.ValueOfDecimal);
                var result = await SelfCheckoutService.UpdateSessionAsync(SelfCheckoutService.CurrentSessionKey, orderNo, shoppingCart);

                CustomerData = RegisterService.CustomerData;
                CurrentShoppingCart = SelfCheckoutService.CurrentShoppingCart;
                await LoadOrderAsync();
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

        public async Task LoadOrderAsync()
        {
            try
            {
                IsBusy = true;
                var payload = new
                {
                    SessionKey = SaleEngineService.LoginData.SessionKey,
                    Attributes = new object[]
                    {
                        new {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = SelfCheckoutService.CurrentShoppingCart
                        }
                    }
                };

                var result = await SaleEngineService.GetOrderAsync(payload);
                await RefreshOrderAsync();
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

        public Task RefreshOrderAsync()
        {
            OrderDetails = SaleEngineService.OrderData?.OrderDetails?.ToObservableCollection();
            MessagingCenter.Send<ViewModelBase>(this, "OrderRefresh");

            IsSelectAllOrder = false;
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
                await SetActionToOrder(payload);
            }
        }

        async Task SetActionToOrder(object payload)
        {
            try
            {
                await SaleEngineService.ActionListItemToOrderAsync(payload);
            }
            catch (Exception ex)
            {
            }
            await RefreshOrderAsync();
        }

        public async Task AddOrderAsync(string barcode)
        {
            try
            {
                IsBusy = true;
                var payload = new
                {
                    SessionKey = SaleEngineService.LoginData.SessionKey,
                    ItemCode = barcode
                };
                var result = await SaleEngineService.AddItemToOrderAsync(payload);
                var success = result.IsCompleted;
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            await RefreshOrderAsync();
        }

        public async Task TestAddOrder()
        {
            var random = new Random();
            var payload = items[random.Next(items.Length)];
            try
            {
                IsBusy = true;
                var result = await SaleEngineService.AddItemToOrderAsync(payload);
                var success = result.IsCompleted;
            }
            catch (Exception ex)
            {
                DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            await RefreshOrderAsync();
        }
    }
}
