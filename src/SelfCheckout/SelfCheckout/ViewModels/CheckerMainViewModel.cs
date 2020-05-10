using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using SelfCheckout.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CheckerMainViewModel : NavigatableViewModelBase
    {
        readonly ISaleEngineService _saleEngineService;
        ContentView _currentView;

        ObservableCollection<TabItem> _tabs;

        public CheckerMainViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigationService)
        {
            _saleEngineService = saleEngineService;
            PackingViewModel = new CheckerPackingViewModel(navigationService, selfCheckoutService, saleEngineService, registerService);
            DeviceStatusViewModel = new DeviceStatusViewModel(navigationService, selfCheckoutService, saleEngineService, registerService);
            SessionHistoryViewModel = new SessionHistoryViewModel(navigationService, selfCheckoutService, saleEngineService, registerService);

            Tabs = new ObservableCollection<TabItem>()
            {
                new TabItem
                {
                    TabText = AppResources.Packing,
                    Page = new CheckerPackingView(){BindingContext = PackingViewModel}
                },
                new TabItem
                {
                    TabText = AppResources.DeviceStatus,
                    Page = new DeviceStatusView(){BindingContext = DeviceStatusViewModel}
                },
                new TabItem
                {
                    TabText = AppResources.SessionHistory,
                    Page = new SessionHistoryView(){BindingContext = SessionHistoryViewModel}
                }
            };

            var firstTab = Tabs.FirstOrDefault();
            firstTab.Selected = true;
            CurrentView = firstTab.Page;
        }

        public CheckerPackingViewModel PackingViewModel { get; }
        public DeviceStatusViewModel DeviceStatusViewModel { get; }
        public SessionHistoryViewModel SessionHistoryViewModel { get; }

        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        public ContentView CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand TabSelectionCommand => new Command<TabItem>((item) =>
        {
            var selectedTab = Tabs.Where(t => t.Selected).FirstOrDefault();
            selectedTab.Selected = false;

            item.Selected = true;
            CurrentView = item.Page;
        });

        public ICommand LogoutCommand => new Command(async () =>
        {
            try
            {
                var result = await NavigationService.ConfirmAsync(AppResources.Logout, AppResources.ConfirmLogout, AppResources.Yes, AppResources.No);
                if (result)
                {
                    await _saleEngineService.LogoutAsync();
                    await GoBackToRootAsync();
                }
            }
            catch { }
        });

        public override Task OnTabSelected(TabItem item)
        {
            return base.OnTabSelected(item);
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            return base.OnTabDeSelected(item);
        }
    }
}
