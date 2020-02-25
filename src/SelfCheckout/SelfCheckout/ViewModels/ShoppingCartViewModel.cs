﻿using Newtonsoft.Json;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
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
    public class ShoppingCartViewModel : ViewModelBase
    {
        object[] items;

        ObservableCollection<OrderDetail> _orderDetails;

        bool _isSelectAllOrder;
        bool _isAnyOrderSelected;

        public ShoppingCartViewModel()
        {
            items = new object[]
            {
                new
                {
                    SessionKey = LoginData.SessionKey,
                    ItemCode = "00008211470207673"
                },
                new
                {
                    SessionKey = LoginData.SessionKey,
                    ItemCode = "00008190415206226"
                }
            };
        }

        public bool IsSelectAllOrder
        {
            get => _isSelectAllOrder;
            set
            {
                _isSelectAllOrder = value;
                RaisePropertyChanged(() => IsSelectAllOrder);

                IsAnyOrderSelected = value;
            }
        }

        public bool IsAnyOrderSelected
        {
            get => _isAnyOrderSelected;
            set
            {
                _isAnyOrderSelected = value;
                RaisePropertyChanged(() => IsAnyOrderSelected);
            }
        }

        public bool IsFirstSelect { get; set; } = true;

        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set
            {
                _orderDetails = value;
                RaisePropertyChanged(() => OrderDetails);
            }
        }

        public ICommand SelectAllOrderCommand => new Command(() =>
        {
            if (OrderDetails == null)
                return;
            IsSelectAllOrder = !IsSelectAllOrder;
            foreach (var order in OrderDetails)
            {
                order.IsSelected = IsSelectAllOrder;
            }
        });

        public ICommand OrderSelectedCommand => new Command<OrderDetail>((order) =>
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

        public ICommand ChangeQtyCommand => new Command<OrderDetail>(async (order) =>
        {
            var qty = order.BillingQuantity.Quantity;
            var payload = new
            {
                SessionKey = LoginData.SessionKey,
                Rows = new string[] { order.Guid },
                ActionItemValue = new
                {
                    Action = "change_qty",
                    Value = $"{qty}"
                }
            };
            await SetActionToOrder(payload);
        });

        public ICommand DeleteOrderCommand => new Command<OrderDetail>(async (order) =>
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
                    var result = await DialogService.ShowConfirmAsync(AppResources.Delete, AppResources.ConfirmDeleteItem, AppResources.Yes, AppResources.No);

                    if (result)
                    {
                        await DeleteItemAsync(selectedOrders);
                    }
                }
            }
        });

        public ICommand RefreshOrderCommand => new Command(async () =>
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
            else
            {
                await TestAddOrder();
            }
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            IsFirstSelect = true;
            return base.OnTabDeSelected(item);
        }

        public async Task LoadOrderAsync()
        {
            try
            {
                IsBusy = true;
                var payload = new
                {
                    SessionKey = LoginData.SessionKey,
                    Attributes = new object[]
                    {
                        new {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = CurrentShoppingCart
                        }
                    }
                };

                var result = await SaleEngineService.GetOrderAsync(payload);
                await RefreshOrderAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Task RefreshOrderAsync()
        {
            OrderDetails = SaleEngineService.OrderData.OrderDetails?.ToObservableCollection();
            MessagingCenter.Send(this, "OrderRefresh");

            IsSelectAllOrder = false;
            return Task.FromResult(true);
        }

        async Task DeleteItemAsync(OrderDetail[] orders)
        {
            if (orders.Any())
            {
                var payload = new
                {
                    SessionKey = LoginData.SessionKey,
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
                    SessionKey = LoginData.SessionKey,
                    ItemCode = barcode
                };
                var result = await SaleEngineService.AddItemToOrderAsync(payload);
                var success = result.IsCompleted;
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            await RefreshOrderAsync();
        }

        async Task TestAddOrder()
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
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            await RefreshOrderAsync();
        }
    }
}
