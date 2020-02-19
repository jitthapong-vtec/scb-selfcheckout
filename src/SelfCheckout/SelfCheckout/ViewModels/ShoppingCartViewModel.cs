using Newtonsoft.Json;
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
        ObservableCollection<OrderDetail> _orderDetails;

        bool _isSelectAllOrder;
        bool _isAnyOrderSelected;

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

        public ICommand DeleteOrderCommand => new Command(async () =>
        {
            var selectedOrder = OrderDetails.Where(o => o.IsSelected).ToList();
            if (selectedOrder.Any())
            {
                var result = await DialogService.ShowConfirmAsync(AppResources.Delete, AppResources.ConfirmDeleteItem, AppResources.Yes, AppResources.No);

                if (result)
                {
                    await DeleteItemAsync();
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
            await LoadOrderAsync();
            await TestAddOrder();
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

        Task RefreshOrderAsync()
        {
            OrderDetails = SaleEngineService.OrderData.OrderDetails?.ToObservableCollection();
            MessagingCenter.Send(this, "OrderLoaded");

            IsSelectAllOrder = false;
            return Task.FromResult(true);
        }

        async Task DeleteItemAsync()
        {
            var selectedOrders = OrderDetails.Where(o => o.IsSelected).ToList();
            if (selectedOrders.Any())
            {
                var payload = new
                {
                    SessionKey = LoginData.SessionKey,
                    Rows = selectedOrders.Select(o => o.Guid).ToArray(),
                    ActionItemValue = new
                    {
                        Action = "cancel",
                        Value = "1"
                    }
                };
                try
                {
                    await SaleEngineService.ActionListItemToOrderAsync(payload);
                }
                catch (Exception ex)
                {
                }
                await RefreshOrderAsync();
            }
        }

        async Task TestAddOrder()
        {
            var payloads = new object[]
            {
                    new
                    {
                        SessionKey = LoginData.SessionKey,
                        ItemCode = "00008211470207673"
                    }
                    ,new
                    {
                        SessionKey = LoginData.SessionKey,
                        ItemCode = "00008190415206226"
                    }
                    //,new
                    //{
                    //    SessionKey = LoginData.SessionKey,
                    //    ItemCode = "00008215930215710"
                    //},new
                    //{
                    //    SessionKey = LoginData.SessionKey,
                    //    ItemCode = "00008215659204729"
                    //},new
                    //{
                    //    SessionKey = LoginData.SessionKey,
                    //    ItemCode = "00008215930215710"
                    //}
            };

            foreach (var payload in payloads)
            {
                try
                {
                    var result = await SaleEngineService.AddItemToOrderAsync(payload);
                    var success = result.IsCompleted;
                }
                catch (Exception ex)
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message);
                }
            }
            await RefreshOrderAsync();
        }
    }
}
