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
        ObservableCollection<OrderDetail> _orderDetails;

        bool _isSelectAllOrder;

        public bool IsSelectAllOrder
        {
            get => _isSelectAllOrder;
            set
            {
                _isSelectAllOrder = value;
                RaisePropertyChanged(() => IsSelectAllOrder);
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

        public ICommand OrderSelectedCommand => new Command<OrderDetail>((order) =>
        {
            order.IsSelected = !order.IsSelected;
        });

        public ICommand RefreshOrderCommand => new Command(async () => await LoadOrderAsync());

        public override async Task OnTabSelected(TabItem item)
        {
            await LoadOrderAsync();
        }

        public async Task LoadOrderAsync()
        {
            try
            {
                IsBusy = true;
                var payload = new
                {
                    SessionKey = "634ffe1e-b61c-4810-a431-0e7cd4f2d581",
                    Attributes = new object[]
                    {
                        new {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = CurrentShoppingCart
                        }
                    }
                };

                await SaleEngineService.GetOrderAsync(payload);

                OrderDetails = SaleEngineService.OrderData.OrderDetails?.ToObservableCollection();

                MessagingCenter.Send(this, "OrderLoaded");
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
    }
}
