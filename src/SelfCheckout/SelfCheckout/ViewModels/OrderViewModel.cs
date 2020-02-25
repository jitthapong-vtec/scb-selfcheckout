using SelfCheckout.Models;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        ObservableCollection<OrderDetail> _orderDetails;

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
            await LoadOrderListAsync();
        }

        async Task LoadOrderListAsync()
        {
            try
            {
                var payload = new
                {
                    SessionKey = LoginData.SessionKey,
                    Attributes = new object[]
                    {
                        new
                        {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = CurrentShoppingCart
                        }
                    },
                    paging = new
                    {
                        pageNo = 1,
                        pageSize = 10
                    },
                    sorting = new object[]
                    {
                        new {
                            sortBy = "headerkey",
                            orderBy = "desc"
                        }
                    }
                };

                var result = await SaleEngineService.GetOrderListAsync(payload);
                if (result.IsCompleted)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
