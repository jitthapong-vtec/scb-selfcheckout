using SelfCheckout.Controls;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.ViewModels.Base;
using SelfCheckout.Views;
using System;
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
        ObservableCollection<Language> _languages;
        ObservableCollection<Payment> _payments;
        ObservableCollection<Currency> _currencies;

        ContentView _currentView;
        Language _languageSelected;
        Currency _currencySelected;
        OrderData _orderData;

        bool _langShowing;
        bool _currencyShowing;
        bool _summaryVisible;
        bool _summaryShowing;

        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabItem>();
            Tabs.Add(new TabItem()
            {
                TabId = 1,
                Icon = "\ue904",
                Title = AppResources.Home,
                TabText = AppResources.Home,
                Page = new HomeView()
            });
            Tabs.Add(new TabItem()
            {
                TabId = 2,
                Icon = "\ue903",
                Title = AppResources.DeviceInfo,
                TabText = AppResources.Device,
                Page = new DeviceView()
            });
            Tabs.Add(new TabItem()
            {
                TabId = 3,
                Icon = "\ue901",
                Title = AppResources.MyCart,
                TabText = AppResources.Shopping,
                Page = new ShoppingCartView(),
                BadgeCount = 0,
                TabType = 1
            });
            Tabs.Add(new TabItem()
            {
                TabId = 4,
                Icon = "\ue900",
                Title = AppResources.Orders,
                TabText = AppResources.Orders,
                Page = new OrderView()
            });
            Tabs.Add(new TabItem()
            {
                TabId = 5,
                Icon = "\ue902",
                Title = AppResources.Profile,
                TabText = AppResources.Profile,
                Page = new ProfileView()
            });

            var firstTab = Tabs.FirstOrDefault();
            firstTab.Selected = true;
            PageTitle = firstTab.Title;

            MessagingCenter.Subscribe<ShoppingCartViewModel>(this, "OrderLoaded", (s) =>
            {
                OrderData = SaleEngineService.OrderData;

                try
                {
                    var tab = Tabs.Where(t => t.TabId == 3).FirstOrDefault();
                    tab.BadgeCount = Convert.ToInt32(OrderData.BillingQty);
                }
                catch { }
            });
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await LoadMasterDataAsync();
            await LoadCurrencyAsync();
        }

        public ICommand TabSelectedCommand => new Command<TabItem>(async (item) => await SelectTabAsync(item));

        public ICommand ScanCommand => new Command<object>(async (data) =>
        {
            try
            {
                var tab = Tabs.Where(t => t.TabId == 3).FirstOrDefault();
                await (tab.Page.BindingContext as ShoppingCartViewModel).AddOrderAsync(data?.ToString());
            }
            catch { }
        });

        public ICommand LanguageTappedCommand => new Command(() =>
        {
            LangShowing = !LangShowing;
        });

        public ICommand CurrencyTappedCommand => new Command(() =>
        {
            CurrencyShowing = !CurrencyShowing;
        });

        public ICommand ShowSummaryCommand => new Command(() =>
        {
            SummaryShowing = !SummaryShowing;
        });

        public ICommand LanguageSelectionCommand => new Command<Language>((lang) =>
        {
            LanguageSelected = lang;
            LangShowing = false;
        });

        public ICommand CurrencySelectionCommand => new Command<Currency>((currency) =>
        {
            CurrencySelected = currency;
            CurrencyShowing = false;
        });

        Task SelectTabAsync(TabItem item)
        {
            if (item.Page is ShoppingCartView || item.Page is OrderView)
                SummaryVisible = true;
            else
                SummaryVisible = false;
            PageTitle = item.Title;
            CurrentView = item.Page;
            item.Selected = true;

            try
            {
                var selectedTab = Tabs.Where(t => t.TabId != item.TabId && t.Selected).FirstOrDefault();
                selectedTab.Selected = false;
            }
            catch { }
            return Task.FromResult(true);
        }

        async Task LoadMasterDataAsync()
        {
            try
            {
                await SelfCheckoutService.LoadLanguageAsync();
                await SelfCheckoutService.LoadPaymentAsync();

                Languages = SelfCheckoutService.Languages?.ToObservableCollection();
                Payments = SelfCheckoutService.Payments?.ToObservableCollection();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message);
            }
        }

        async Task LoadCurrencyAsync()
        {
            try
            {
                var payload = new
                {
                    branch_no = SelfCheckoutService.AppConfig.BranchNo
                };
                await SaleEngineService.LoadCurrencyAsync(payload);
                Currencies = SaleEngineService.Currencies?.ToObservableCollection();
                CurrencySelected = SaleEngineService.Currencies?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message);
            }
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

        public ObservableCollection<Language> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                RaisePropertyChanged(() => Languages);
            }
        }

        public ObservableCollection<Currency> Currencies
        {
            get => _currencies;
            set
            {
                _currencies = value;
                RaisePropertyChanged(() => Currencies);
            }
        }

        public ObservableCollection<Payment> Payments
        {
            get => _payments;
            set
            {
                _payments = value;
                RaisePropertyChanged(() => Payments);
            }
        }

        public OrderData OrderData
        {
            get => _orderData;
            set
            {
                _orderData = value;
                RaisePropertyChanged(() => OrderData);
            }
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

        public bool SummaryShowing
        {
            get => _summaryShowing;
            set
            {
                _summaryShowing = value;
                RaisePropertyChanged(() => SummaryShowing);
            }
        }

        public bool SummaryVisible
        {
            get => _summaryVisible;
            set
            {
                _summaryVisible = value;
                RaisePropertyChanged(() => SummaryVisible);
            }
        }

        public Language LanguageSelected
        {
            get => _languageSelected;
            set
            {
                _languageSelected = value;
                RaisePropertyChanged(() => LanguageSelected);
            }
        }

        public Currency CurrencySelected
        {
            get => _currencySelected;
            set
            {
                _currencySelected = value;
                RaisePropertyChanged(() => CurrencySelected);
            }
        }

        public bool CurrencyShowing
        {
            get => _currencyShowing;
            set
            {
                _currencyShowing = value;
                RaisePropertyChanged(() => CurrencyShowing);

                if (value)
                    LangShowing = false;
            }
        }

        public bool LangShowing
        {
            get => _langShowing;
            set
            {
                _langShowing = value;
                RaisePropertyChanged(() => LangShowing);

                if (value)
                    CurrencyShowing = false;
            }
        }
    }
}
