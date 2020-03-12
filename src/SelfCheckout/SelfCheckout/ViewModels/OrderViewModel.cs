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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class OrderViewModel : OrderViewModelBase
    {
        bool _summaryShowing;

        public OrderViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) 
            : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
            Tabs = new ObservableCollection<SimpleSelectedItem>()
            {
                new SimpleSelectedItem()
                {
                    Text1 = "",
                    Selected = true,
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = "",
                    Arg1 = 2
                }
            };

            MessagingCenter.Subscribe<MainViewModel>(this, "CurrencyChanged", async (s) =>
            {
                await GetOrderListAsync(SaleEngineService.LoginData.SessionKey, SelfCheckoutService.StartedShoppingCard);
            });
        }

        public ICommand TabSelectedCommand => new DelegateCommand<SimpleSelectedItem>((item) =>
        {
            var seletedItem = Tabs.Where(t => t.Selected).FirstOrDefault();
            seletedItem.Selected = false;

            item.Selected = true;
        });

        public ICommand CustomerFilterTappedCommand => new DelegateCommand(() =>
        {

        });

        public ICommand ShowSummaryCommand => new DelegateCommand(() =>
        {
            SummaryShowing = !SummaryShowing;
        });

        public bool SummaryShowing
        {
            get => _summaryShowing;
            set => SetProperty(ref _summaryShowing, value);
        }

        public override async Task OnTabSelected(TabItem item)
        {
            await GetOrderListAsync(SaleEngineService.LoginData.SessionKey, SelfCheckoutService.StartedShoppingCard);
        }
    }
}
