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

        public override async Task OnTabSelected(TabItem item)
        {
            await LoadOrderAsync();
        }

        public async Task LoadOrderAsync()
        {
            try
            {
                //var payload = new
                //{
                //    SessionKey = "634ffe1e-b61c-4810-a431-0e7cd4f2d581",
                //    Attributes = new object[]
                //    {
                //        new {
                //            GROUP = "tran_no",
                //            CODE = "shopping_card",
                //            valueOfString = CurrentShoppingCart
                //        }
                //    }
                //};
                //var result = await SaleEngineService.GetOrderAsync(payload);
                //var data = result.Data;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "SelfCheckout.Resources.order_list.json";

                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var json = await reader.ReadToEndAsync();
                        var result = JsonConvert.DeserializeObject<ApiResultData<List<OrderData>>>(json);

                        var orderData = result.Data.FirstOrDefault();
                        OrderDetails = orderData.OrderDetails.ToObservableCollection();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
        }
    }
}
