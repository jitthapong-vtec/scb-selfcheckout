using SelfCheckout.Models;
using SelfCheckout.Resources;
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

        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;
                RaisePropertyChanged(() => Tabs);
            }
        }

        public ICommand TabSelectionCommand => new Command<TabItem>((item) =>
        {
            var selectedTab = Tabs.Where(t => t.Selected).FirstOrDefault();
            selectedTab.Selected = false;

            item.Selected = true;
        });

        public CheckerMainViewModel()
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
