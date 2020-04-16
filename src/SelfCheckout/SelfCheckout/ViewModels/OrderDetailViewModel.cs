using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class OrderDetailViewModel : NavigatableViewModelBase
    {
        OrderDetail _orderDetail;

        public OrderDetailViewModel(INavigationService navigatinService) : base(navigatinService)
        {
        }

        public OrderDetail OrderDetail
        {
            get => _orderDetail;
            set => SetProperty(ref _orderDetail, value);
        }

        public ICommand ShowImageDetailCommand => new DelegateCommand(async() =>
        {
            var parameter = new NavigationParameters()
            {
                {"ImageUrl", OrderDetail.ImageUrl}
            };
            await NavigationService.ShowDialogAsync<INavigationParameters>("ProductImageDetailDialog", parameter);
        });

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var orderDetail = parameters.GetValue<OrderDetail>("OrderDetail");
            if (orderDetail != null)
                OrderDetail = orderDetail;
        }
    }
}
