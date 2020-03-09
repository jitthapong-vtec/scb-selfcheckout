using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
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
        ObservableCollection<TabItem> _tabs;

        public CheckerMainViewModel(INavigationService navigatinService, IDialogService dialogService) : base(navigatinService, dialogService)
        {
            Tabs = new ObservableCollection<TabItem>()
            {
                new TabItem
                {
                    TabText = AppResources.Packing,
                },
                new TabItem
                {
                    TabText = AppResources.DeviceStatus
                },
                new TabItem
                {
                    TabText = AppResources.SessionHistory
                }
            };

            var firstTab = Tabs.FirstOrDefault();
            firstTab.Selected = true;
        }

        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        public ICommand TabSelectionCommand => new Command<TabItem>((item) =>
        {
            var selectedTab = Tabs.Where(t => t.Selected).FirstOrDefault();
            selectedTab.Selected = false;

            item.Selected = true;
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
