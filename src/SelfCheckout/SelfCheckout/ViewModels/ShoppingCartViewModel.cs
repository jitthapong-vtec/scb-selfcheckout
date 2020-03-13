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
        ObservableCollection<OrderDetail> _orderDetails;

        CustomerData _customerData;

        string _currentShoppingCard;

        bool _isSelectAllOrder;
        bool _isAnyOrderSelected;
        bool _isChangeShoppingCardShowing;

        public ShoppingCartViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, saleEngineService, selfCheckoutService, registerService)
        {
            CustomerData = RegisterService.CustomerData;
            CurrentShoppingCard = SelfCheckoutService.CurrentShoppingCard;

            MessagingCenter.Subscribe<MainViewModel>(this, "CurrencyChanged", async (s) =>
            {
                await RefreshOrderAsync();
            });
        }

        public ICommand ToggleChangeShoppingCardCommand => new DelegateCommand(() =>
        {
            IsChangeShoppingCardShowing = !IsChangeShoppingCardShowing;
        });

        public ICommand ChangeShoppingCardCommand => new DelegateCommand(() =>
        {
            IsChangeShoppingCardShowing = false;
            DialogService.ShowDialog("ShoppingCardInputDialog", null, async (dialogResult) =>
            {
                var shoppingCard = dialogResult.Parameters.GetValue<string>("ShoppingCard");
                if (!string.IsNullOrEmpty(shoppingCard))
                    await ValidateShoppingCardAsync(shoppingCard);
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
                await LoadOrderAsync();

                IsFirstSelect = false;
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
            await LoadOrderAsync();
        }

        public async Task LoadOrderAsync()
        {
            try
            {
                var appConfig = SelfCheckoutService.AppConfig;
                var loginResult = await SaleEngineService.LoginAsync(appConfig.UserName, appConfig.Password);
                SaleEngineService.LoginData = loginResult.Data;

                IsBusy = true;
                var payload = new
                {
                    SessionKey = SaleEngineService.LoginData.SessionKey,
                    Attributes = new object[]
                    {
                        new {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = CurrentShoppingCard
                        }
                    },
                    paging = new
                    {
                        pageNo = 1,
                        pageSize = 100
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

                await SaleEngineService.GetOrderAsync(payload);
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
            MessagingCenter.Send(this, "OrderRefresh");

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
            var payload = new
            {
                SessionKey = SaleEngineService.LoginData.SessionKey,
                ItemCode = "00008211470207673"
            };

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
