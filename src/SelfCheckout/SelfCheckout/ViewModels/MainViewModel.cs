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
        Payment _paymentSelected;
        OrderData _orderData;

        string _paymentBarCode;

        bool _langShowing;
        bool _currencyShowing;
        bool _summaryVisible;
        bool _summaryShowing;
        bool _isSummaryOrder;
        bool _onProcessPayment;
        bool _paymentSelectionShowing;
        bool _paymentProcessShowing;

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
            CurrentView = firstTab.Page;

            MessagingCenter.Subscribe<ShoppingCartViewModel>(this, "OrderRefresh", (s) =>
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

        public ICommand PaymentMethodTappedCommand => new Command(() =>
        {
            PaymentSelectionShowing = !PaymentSelectionShowing;
        });

        public ICommand PaymentSelectionCommand => new Command<Payment>((payment) =>
        {
            PaymentSelectionShowing = false;
            PaymentSelected = payment;
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

        public ICommand ScanPaymentCommand => new Command<object>(async (type) =>
        {
            if (Convert.ToInt32(type) == 1)
            {
                var task = new TaskCompletionSource<string>();
                await NavigationService.PushModalAsync<BarcodeScanViewModel, string>(null, task);
                var result = await task.Task;
                if (!string.IsNullOrEmpty(result))
                {
                    PaymentBarcode = result;
                    await PaymentAsync();
                }
            }
            else
            {
                //TODO: hardware scan
            }
        });

        public ICommand SimulatePaymentProcessCancelCommand => new Command(() =>
        {
            PaymentProcessShowing = false;
        });

        public ICommand CheckoutCommand => new Command(() =>
        {
            if (OnProcessPayment)
            {
                PaymentProcessShowing = true;
            }
            else
            {
                OnProcessPayment = true;
            }
        });

        Task SelectTabAsync(TabItem item)
        {
            if (item.Page is ShoppingCartView || item.Page is OrderView)
            {
                SummaryVisible = true;
                if (item.Page is OrderView)
                {
                    OnProcessPayment = false;
                    IsOrderSummary = true;
                }
                else
                {
                    IsOrderSummary = false;
                }
            }
            else
            {
                SummaryVisible = false;
            }
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

                LanguageSelected = SelfCheckoutService.Languages.FirstOrDefault();
                PaymentSelected = SelfCheckoutService.Payments.FirstOrDefault();
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

        async Task PaymentAsync()
        {
            try
            {
                IsBusy = true;
                var walletRequestPayload = new
                {
                    barcode = PaymentBarcode,
                    machine_ip = LoginData.UserInfo.MachineEnv.MachineIp,
                    branch_no = AppConfig.BranchNo
                };
                var walletResult = await SaleEngineService.GetWalletTypeFromBarcodeAsync(walletRequestPayload);
                if (walletResult.IsCompleted)
                {
                    //var wallet = walletResult.Data;
                    //var netAmount = OrderData.BillingAmount.NetAmount.CurrAmt;
                    //var paymentRequestPayload = new
                    //{
                    //    SessionKey = LoginData.SessionKey,
                    //    OrderGuid = OrderData.Guid,
                    //    Payment = new
                    //    {
                    //        PaymentCode = PaymentSelected.MethodCode,
                    //        PaymentType = wallet.WalletType,
                    //        RefNo = PaymentBarcode,
                    //        URLService = wallet.WalletagentMaster.Wsurl,
                    //        WalletBarcode = PaymentBarcode,
                    //        WalletMerchantID = wallet.WalletagentMaster.MerchantId,
                    //        GatewayId = PaymentSelected.GatewayId,
                    //        PaymentAmounts = new
                    //        {
                    //            CurrAmt = netAmount,
                    //            CrrCode = new
                    //            {
                    //                Code = CurrencySelected.CurrCode,
                    //                Desc = CurrencySelected.CurrDesc
                    //            },
                    //            CurrRate = CurrencySelected.CurrRate,
                    //            BaseCurrCode = new
                    //            {
                    //                Code = BaseCurrency.CurrCode,
                    //                Desc = BaseCurrency.CurrDesc
                    //            },
                    //            BaseCurrRate = 1,
                    //            BaseCurrAmt = netAmount
                    //        },
                    //        Transaction = new
                    //        {
                    //            TransactionGroup = 2,
                    //            TransactionType = 1,
                    //            PartnerId = wallet.WalletagentMaster.PartnertypeId,
                    //            PartnerType = wallet.WalletagentMaster.PartnertypeId,
                    //            LastStatus = 1,
                    //            CurrentStatus = 1,
                    //            Movements = new[]
                    //            {
                    //                new
                    //                {
                    //                    TransactionMovementType = 1,
                    //                    Amount = netAmount,
                    //                    Description = "",
                    //                    Status = 1
                    //                }
                    //            }
                    //        },
                    //        ChangeAmounts = new
                    //        {
                    //            CurrAmt = netAmount,
                    //            CrrCode = new
                    //            {
                    //                Code = CurrencySelected.CurrCode,
                    //                Desc = CurrencySelected.CurrDesc
                    //            },
                    //            CurrRate = CurrencySelected.CurrRate,
                    //            BaseCurrCode = new
                    //            {
                    //                Code = BaseCurrency.CurrCode,
                    //                Desc = BaseCurrency.CurrDesc
                    //            },
                    //            BaseCurrRate = 1,
                    //            BaseCurrAmt = netAmount
                    //        }
                    //    }
                    //};

                    //var paymentResult = await SaleEngineService.AddPaymentToOrderAsync(paymentRequestPayload);
                    //if (paymentResult.IsCompleted)
                    //{
                    //    //TODO: Show thank you popup and goto order tab
                    //}
                    //else
                    //{
                    //    await DialogService.ShowAlertAsync(AppResources.Opps, paymentResult.DefaultMessage, AppResources.Close);
                    //}
                }
                else
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, walletResult.DefaultMessage, AppResources.Close);
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
                PaymentProcessShowing = false;
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

        public Payment PaymentSelected
        {
            get => _paymentSelected;
            set
            {
                _paymentSelected = value;
                RaisePropertyChanged(() => PaymentSelected);
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

        public string PaymentBarcode
        {
            get => _paymentBarCode;
            set
            {
                _paymentBarCode = value;
                RaisePropertyChanged(() => PaymentBarcode);
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

        public bool IsOrderSummary
        {
            get => _isSummaryOrder;
            set
            {
                _isSummaryOrder = value;
                RaisePropertyChanged(() => IsOrderSummary);
            }
        }

        public bool OnProcessPayment
        {
            get => _onProcessPayment;
            set
            {
                _onProcessPayment = value;
                RaisePropertyChanged(() => OnProcessPayment);
            }
        }

        public bool PaymentProcessShowing
        {
            get => _paymentProcessShowing;
            set
            {
                _paymentProcessShowing = value;
                RaisePropertyChanged(() => PaymentProcessShowing);
            }
        }

        public bool PaymentSelectionShowing
        {
            get => _paymentSelectionShowing;
            set
            {
                _paymentSelectionShowing = value;
                RaisePropertyChanged(() => PaymentSelectionShowing);
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

        public Currency BaseCurrency
        {
            get
            {
                return Currencies.Where(c => c.CurrCode == "THB").FirstOrDefault() ?? new Currency
                {
                    CurrCode = "THB",
                    CurrDesc = "THB",
                    CurrRate = 1
                };
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
