using SelfCheckout.Controls;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Identity;
using SelfCheckout.ViewModels.Base;
using SelfCheckout.Views;
using System.Collections.Generic;
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

        ContentView _currentView;

        bool _langShowing;

        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabItem>();
            Tabs.Add(new TabItem()
            {
                Icon = "\ue904",
                Title = AppResources.Home,
                TabText = AppResources.Home,
                Page = new HomeView()
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue903",
                Title = AppResources.DeviceInfo,
                TabText = AppResources.Device,
                Page = new DeviceView()
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue901",
                Title = AppResources.MyCart,
                TabText = AppResources.Shopping,
                Page = new ShoppingCartView(),
                BadgeCount = 0,
                TabType = 1
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue900",
                Title = AppResources.Orders,
                TabText = AppResources.Orders,
                Page = new OrderView()
            });
            Tabs.Add(new TabItem()
            {
                Icon = "\ue902",
                Title = AppResources.Profile,
                TabText = AppResources.Profile,
                Page = new ProfileView()
            });

            var firstTab = Tabs.FirstOrDefault();
            firstTab.Selected = true;
            PageTitle = firstTab.Title;
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }

        public ICommand TabSelectedCommand => new Command<TabItem>(async (item) => await SelectTabAsync(item));

        public ICommand LanguageTappedCommand => new Command(() =>
        {
            LangShowing = !LangShowing;
        });

        public ICommand LanguageSelectionCommand => new Command(() =>
        {
            LangShowing = false;
        });

        Task SelectTabAsync(TabItem item)
        {
            var selectedTab = Tabs.Where(t => t.Selected).FirstOrDefault();
            selectedTab.Selected = false;

            item.Selected = true;
            PageTitle = item.Title;
            CurrentView = item.Page;
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

        public IList<Language> Languages
        {
            get => MasterDataService.Languages;
        }

        public ContentView CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                RaisePropertyChanged(() => CurrentView);
            }
        }

        public bool LangShowing
        {
            get => _langShowing;
            set
            {
                _langShowing = value;
                RaisePropertyChanged(() => LangShowing);
            }
        }
    }
}
