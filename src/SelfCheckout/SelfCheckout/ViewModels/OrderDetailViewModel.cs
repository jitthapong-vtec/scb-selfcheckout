using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        OrderDetail _orderDetail;

        public OrderDetailViewModel(INavigationService navigatinService, IDialogService dialogService) : base(navigatinService, dialogService)
        {
        }

        public OrderDetail OrderDetail
        {
            get => _orderDetail;
            set => SetProperty(ref _orderDetail, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            OrderDetail = parameters.GetValue<OrderDetail>("OrderDetail");
        }
    }
}
