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
    public class CheckerMainViewModel : ViewModelBase
    {
        ContentView _currentView;

        ObservableCollection<TabItem> _tabs;

        public CheckerMainViewModel(IDialogService dialogService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService) : base(dialogService)
        {
            PackingViewModel = new CheckerPackingViewModel(dialogService, selfCheckoutService, saleEngineService, registerService);
            DeviceStatusViewModel = new DeviceStatusViewModel(dialogService, selfCheckoutService, saleEngineService, registerService);
            SessionHistoryViewModel = new SessionHistoryViewModel(dialogService, selfCheckoutService, saleEngineService, registerService);

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
