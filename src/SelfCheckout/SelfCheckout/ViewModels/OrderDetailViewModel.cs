using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

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

        public ICommand ShowImageDetailCommand => new DelegateCommand(() =>
        {
            var parameter = new DialogParameters()
            {
                {"ImageUrl", OrderDetail.ImageUrl}
            };
            DialogService.ShowDialog("ProductImageDetailDialog", parameter);
        });

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            OrderDetail = parameters.GetValue<OrderDetail>("OrderDetail");
        }
    }
}
