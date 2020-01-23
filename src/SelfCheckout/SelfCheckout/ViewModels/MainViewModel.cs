﻿using SelfCheckout.Resources;
using SelfCheckout.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        ObservableCollection<TabItem> _tabs;

        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabItem>();
            Tabs.Add(new TabItem()
            {
                Icon = "\ue904",
                Title = AppResources.Home
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue903",
                Title = AppResources.Device
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue901",
                Title = AppResources.Shopping,
                Selected = true,
                TabType = 1
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue900",
                Title = AppResources.Orders
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue902",
                Title = AppResources.Profile
            });
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }

        public ICommand TabSelectedCommand => new Command<TabItem>(async (item) => await SelectTabAsync(item));

        Task SelectTabAsync(TabItem item)
        {
            var selectedTab = Tabs.Where(t => t.Selected).FirstOrDefault();
            selectedTab.Selected = false;

            item.Selected = true;
            return Task.FromResult(true);
        }

        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;
                RaisePropertyChanged(() => Tabs);
            }
        }
    }
}
