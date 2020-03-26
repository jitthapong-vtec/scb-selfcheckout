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

        public CheckerMainViewModel(INavigationService navigatinService, IDialogService dialogService) : base(navigatinService, dialogService)
        {
            Tabs = new ObservableCollection<TabItem>()
            {
                new TabItem
                {
                    TabText = AppResources.Packing,
                    Page = new CheckerPackingView()
                },
                new TabItem
                {
                    TabText = AppResources.DeviceStatus,
                    Page = new DeviceStatusView()
                },
                new TabItem
                {
                    TabText = AppResources.SessionHistory,
                    Page = new SessionHistoryView()
                }
            };

            var firstTab = Tabs.FirstOrDefault();
            firstTab.Selected = true;
            CurrentView = firstTab.Page;
        }

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
