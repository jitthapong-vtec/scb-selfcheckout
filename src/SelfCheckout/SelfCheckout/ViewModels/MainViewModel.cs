using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Exceptions;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.PimCore;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using SelfCheckout.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class MainViewModel : NavigatableViewModelBase
    {
        object _lockProcess = new object();

        ISaleEngineService _saleEngineService;
        ISelfCheckoutService _selfCheckoutService;

        ObservableCollection<TabItem> _tabs;
        ObservableCollection<Language> _languages;
        ObservableCollection<Currency> _currencies;
        ObservableCollection<Payment> _payments;

        ContentView _currentView;
        Language _languageSelected;
        Currency _currencySelected;
        Payment _paymentSelected;
        OrderData _orderData;

        int _selectedTabId;
        int _paymentCountdownTimer;

        string _paymentBarCode;
        string _couponCode;
        string _checkoutButtonText;

        bool _systemViewVisible;
        bool _summaryVisible;
        bool _summaryShowing;
        bool _langShowing;
        bool _currencyShowing;
        bool _isBeingPaymentProcess;
        bool _isPaymentProcessing;
        bool _paymentSelectionShowing;
        bool _paymentInputShowing;
        bool _isChangingShoppingCard;

        public MainViewModel(INavigationService navigatinService, IDialogService dialogService,
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService,
            IRegisterService registerService, IPimCoreService pimCoreService) : base(navigatinService, dialogService)
        {
            _saleEngineService = saleEngineService;
            _selfCheckoutService = selfCheckoutService;

            HomeViewModel = new HomeViewModel(dialogService, selfCheckoutService, pimCoreService);
            HomeViewModel.ShowSystemView = () => SystemViewVisible = true;

            DeviceViewModel = new DeviceViewModel(dialogService, saleEngineService, registerService);

            ShoppingCartViewModel = new ShoppingCartViewModel(dialogService, selfCheckoutService, saleEngineService, registerService);
            ShoppingCartViewModel.ReloadOrderDataAsync = LoadOrderAsync;
            ShoppingCartViewModel.RefreshSummary = RefreshSummary;
            ShoppingCartViewModel.ShowOrderDetail = async (order) => await NavigationService.NavigateAsync("OrderDetailView", new NavigationParameters() { { "OrderDetail", order } });
            ShoppingCartViewModel.ShowCameraScanner = ShowCameraScannerAsync;
            ShoppingCartViewModel.ShoppingCardChanging = (isChanging) => _isChangingShoppingCard = isChanging;

            OrderViewModel = new OrderViewModel(dialogService, selfCheckoutService, saleEngineService, registerService);
            OrderViewModel.GoBackToRootAsync = GoBackToRootAsync;

            ProfileViewModel = new ProfileViewModel(dialogService, selfCheckoutService);

            TutorialViewModel = new TutorialViewModel(dialogService, selfCheckoutService, pimCoreService);

            CouponInputViewModel = new CouponInputViewModel(dialogService);
            CouponInputViewModel.OpenCameraScannerAsync = ShowCameraScannerAsync;
            CouponInputViewModel.SetCouponAsync = AddCouponAsync;
        }

        private void RefreshTab()
        {
            Tabs = new ObservableCollection<TabItem>();
            Tabs.Add(new TabItem()
            {
                TabId = 1,
                Icon = "\ue904",
                Title = AppResources.Home,
                TabText = AppResources.Home,
                Page = new HomeView() { BindingContext = HomeViewModel }
            });
            Tabs.Add(new TabItem()
            {
                TabId = 2,
                Icon = "\ue903",
                Title = AppResources.DeviceInfo,
                TabText = AppResources.Device,
                Page = new DeviceView() { BindingContext = DeviceViewModel }
            });
            Tabs.Add(new TabItem()
            {
                TabId = 3,
                Icon = "\ue901",
                Title = AppResources.MyCart,
                TabText = AppResources.Shopping,
                Page = new ShoppingCartView() { BindingContext = ShoppingCartViewModel },
                BadgeCount = 0,
                TabType = 1
            });
            Tabs.Add(new TabItem()
            {
                TabId = 4,
                Icon = "\ue900",
                Title = AppResources.Orders,
                TabText = AppResources.Orders,
                Page = new OrderView() { BindingContext = OrderViewModel }
            });
            Tabs.Add(new TabItem()
            {
                TabId = 5,
                Icon = "\ue902",
                Title = AppResources.Profile,
                TabText = AppResources.Profile,
                Page = new ProfileView() { BindingContext = ProfileViewModel }
            });

            if (_selectedTabId == 0)
            {
                _selectedTabId = 1;
            }
            try
            {
                var tab = Tabs.Where(t => t.TabId == _selectedTabId).FirstOrDefault();
                tab.Selected = true;
                PageTitle = tab.Title;
                CurrentView = tab.Page;
            }
            catch { }
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await LoadMasterDataAsync();
            await LoadCurrencyAsync();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            await LoadOrderAsync();

            if (!string.IsNullOrEmpty(parameters.GetValue<string>("ScanData")))
            {
                var scanData = parameters.GetValue<string>("ScanData");
                if (CouponInputViewModel.CouponInputViewVisible)
                    CouponInputViewModel.CouponCode = scanData;
                else
                    MessagingCenter.Send(this, "ReceiveShoppingCardFromScanner", scanData);
            }
        }

        public ICommand TabSelectedCommand => new Command<TabItem>(async (item) => await SelectTabAsync(item));

        public ICommand ScanCommand => new Command<object>(async (data) =>
        {
            if (IsBeingPaymentProcess)
            {
                PaymentBarcode = data?.ToString();
                if (!string.IsNullOrEmpty(PaymentBarcode))
                    await WalletPaymentAsync();
            }
            else
            {
                if (_isChangingShoppingCard)
                {
                    MessagingCenter.Send(this, "ReceiveShoppingCardFromScanner", data?.ToString());
                }
                else if (CouponInputViewModel.CouponInputViewVisible)
                {
                    CouponInputViewModel.CouponCode = data?.ToString();
                }
                else
                {
                    if (CurrentView is ShoppingCartView)
                        await AddOrderAsync(data?.ToString());
                }
            }
        });

        public ICommand ScanCouponCommand => new Command(async () =>
        {
            try
            {
                if (OrderData.TotalBillingAmount.NetAmount.CurrAmt <= 0)
                    return;
            }
            catch
            {
                return;
            }

            if (!string.IsNullOrEmpty(CouponCode))
            {
                await DeleteCouponAsync();
            }
            else
            {
                CouponInputViewModel.CouponInputViewVisible = true;
            }
        });

        public ICommand ShowInfoCommand => new Command(() =>
        {
            TutorialViewModel.TutorialViewVisible = !TutorialViewModel.TutorialViewVisible;
        });

        public ICommand LanguageTappedCommand => new Command(() =>
        {
            LangShowing = !LangShowing;
        });

        public ICommand CurrencyTappedCommand => new Command(() =>
        {
            CurrencyShowing = !CurrencyShowing;
        });

        public ICommand LanguageSelectionCommand => new Command<Language>((lang) =>
        {
            LanguageSelected = lang;
            LangShowing = false;
        });

        public ICommand CurrencySelectionCommand => new Command<Currency>(async (currency) =>
        {
            CurrencySelected = currency;
            CurrencyShowing = false;

            await ChangeCurrency();
        });

        public ICommand ShowSummaryCommand => new Command(() =>
        {
            SummaryShowing = !SummaryShowing;
            if (SummaryShowing == false)
                IsBeingPaymentProcess = false;
        });

        public ICommand HideSystemCommand => new Command(() =>
        {
            SystemViewVisible = false;
        });

        public ICommand LogoutCommand => new Command(async () =>
        {
            try
            {
                var result = await DialogService.ConfirmAsync(AppResources.Logout, AppResources.ConfirmLogout, AppResources.Yes, AppResources.No);
                if (result)
                {
                    await _saleEngineService.LogoutAsync();
                    await GoBackToRootAsync();
                }
            }
            catch { }
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

        public ICommand ScanPaymentCommand => new Command<object>((type) =>
            {
                if (IsPaymentProcessing)
                    return;

                if (Convert.ToInt32(type) == 1)
                {
                    DialogService.ShowDialog("BarcodeScanDialog", null, async (dialogResult) =>
                    {
                        var result = dialogResult.Parameters.GetValue<string>("ScanData");
                        if (!string.IsNullOrEmpty(result))
                        {
                            PaymentBarcode = result;
                            await WalletPaymentAsync();
                        }
                    });
                }
                else
                {
                    MessagingCenter.Send<ViewModelBase>(this, "RequestHWScanner");
                }
            });

        public ICommand CancelPaymentProcessCommand => new Command(() =>
        {
            if (IsPaymentProcessing)
                return;
            PaymentInputShowing = false;
            IsBeingPaymentProcess = false;
        });

        public ICommand CheckoutCommand => new Command(async () =>
        {
            try
            {
                if (!_saleEngineService.OrderData.OrderDetails.Any())
                    return;
            }
            catch { return; }

            if (!IsBeingPaymentProcess)
            {
                await CheckoutAsync();
            }
            else
            {
                if (!PaymentSelected.IsAlipay)
                {
                    var result = await DialogService.ShowDialogAsync("PromptPayQrDialog", null);
                    var promptPayResult = result.Parameters.GetValue<PromptPayResult>("PromptPayResult");
                    if (promptPayResult == null)
                    {
                        PaymentInputShowing = false;
                        IsBeingPaymentProcess = false;
                        PaymentSelected = _selfCheckoutService.Payments.FirstOrDefault();
                    }
                    else
                    {
                        await QRPaymentAsync(promptPayResult);
                    }
                }
                else
                {
                    PaymentInputShowing = true;
                }
            }
        });

        public HomeViewModel HomeViewModel { get; }

        public DeviceViewModel DeviceViewModel { get; }

        public ShoppingCartViewModel ShoppingCartViewModel { get; }

        public OrderViewModel OrderViewModel { get; }

        public ProfileViewModel ProfileViewModel { get; }

        public TutorialViewModel TutorialViewModel { get; }

        public CouponInputViewModel CouponInputViewModel { get; }

        public AppConfig AppConfig { get => _selfCheckoutService.AppConfig; }

        public LoginData LoginData { get => _saleEngineService.LoginData; }

        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        public ObservableCollection<Language> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        public ObservableCollection<Currency> Currencies
        {
            get => _currencies;
            set => SetProperty(ref _currencies, value);
        }

        public ContentView CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value, () =>
            {
                if (value is ShoppingCartView)
                {
                    SummaryVisible = true;
                }
                else
                {
                    SummaryVisible = false;

                    if (value is OrderView)
                    {
                        IsBeingPaymentProcess = false;
                    }
                }
            });
        }

        public ObservableCollection<Payment> Payments
        {
            get => _payments;
            set => SetProperty(ref _payments, value);
        }

        public Payment PaymentSelected
        {
            get => _paymentSelected;
            set => SetProperty(ref _paymentSelected, value);
        }

        public int PaymentCountdownTimer
        {
            get => _paymentCountdownTimer;
            set => SetProperty(ref _paymentCountdownTimer, value);
        }

        public string PaymentBarcode
        {
            get => _paymentBarCode;
            set => SetProperty(ref _paymentBarCode, value);
        }

        public bool IsPaymentProcessing
        {
            get => _isPaymentProcessing;
            set => SetProperty(ref _isPaymentProcessing, value);
        }

        public bool IsBeingPaymentProcess
        {
            get => _isBeingPaymentProcess;
            set => SetProperty(ref _isBeingPaymentProcess, value, () =>
            {
                if (value)
                    CheckoutButtonText = AppResources.Pay;
                else
                    CheckoutButtonText = AppResources.Checkout;
            });
        }

        public bool PaymentSelectionShowing
        {
            get => _paymentSelectionShowing;
            set => SetProperty(ref _paymentSelectionShowing, value);
        }

        public bool PaymentInputShowing
        {
            get => _paymentInputShowing;
            set => SetProperty(ref _paymentInputShowing, value);
        }

        public bool SummaryShowing
        {
            get => _summaryShowing;
            set => SetProperty(ref _summaryShowing, value);
        }

        public bool SystemViewVisible
        {
            get => _systemViewVisible;
            set => SetProperty(ref _systemViewVisible, value);
        }

        public bool SummaryVisible
        {
            get => _summaryVisible;
            set => SetProperty(ref _summaryVisible, value);
        }

        public string CouponCode
        {
            get => _couponCode;
            set => SetProperty(ref _couponCode, value);
        }

        public string CheckoutButtonText
        {
            get => _checkoutButtonText;
            set => SetProperty(ref _checkoutButtonText, value);
        }

        public OrderData OrderData
        {
            get => _orderData;
            set => SetProperty(ref _orderData, value);
        }

        public Language LanguageSelected
        {
            get => _languageSelected;
            set => SetProperty(ref _languageSelected, value, async () =>
            {
                _selfCheckoutService.CurrentLanguage = value;

                if (value.LangCode == "EN")
                    GlobalSettings.Instance.CountryCode = "en-US";
                else if (value.LangCode == "TH")
                    GlobalSettings.Instance.CountryCode = "th-TH";
                else if (value.LangCode == "ZH")
                    GlobalSettings.Instance.CountryCode = "zh-Hans";
                GlobalSettings.Instance.InitLanguage();

                MessagingCenter.Send(this, "LanguageChange");
                await TutorialViewModel.LoadImageAsset();
                RefreshTab();
            });
        }

        public Currency CurrencySelected
        {
            get => _currencySelected;
            set => SetProperty(ref _currencySelected, value, () =>
            {
                _saleEngineService.CurrencySelected = value;
            });
        }

        public bool CurrencyShowing
        {
            get => _currencyShowing;
            set
            {
                SetProperty(ref _currencyShowing, value, () =>
                {
                    if (value)
                        LangShowing = false;
                });
            }
        }

        public bool LangShowing
        {
            get => _langShowing;
            set
            {
                SetProperty(ref _langShowing, value, () =>
                {
                    if (value)
                        CurrencyShowing = false;
                });
            }
        }

        async Task SelectTabAsync(TabItem item)
        {
            PageTitle = item.Title;
            CurrentView = item.Page;
            item.Selected = true;

            _selectedTabId = item.TabId;

            try
            {
                // Reset selected tab
                var selectedTab = Tabs.Where(t => t.TabId != item.TabId && t.Selected).FirstOrDefault();
                selectedTab.Selected = false;
            }
            catch { }

            if (!(item.Page is OrderView))
            {
                await CheckSessionAlreadyEndAsync();
            }
        }

        async Task CheckSessionAlreadyEndAsync()
        {
            try
            {
                var sessionData = await _selfCheckoutService.GetSessionDetialAsync(_selfCheckoutService.BorrowSessionKey);
                if (sessionData.SessionStatus.SessionCode == "END")
                {
                    await _saleEngineService.LogoutAsync();
                    await GoBackToRootAsync();
                }
            }
            catch { }
        }

        async Task ShowCameraScannerAsync()
        {
            await NavigationService.NavigateAsync("CameraScannerView");
        }

        async Task ChangeCurrency()
        {
            try
            {
                var payload = new
                {
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                    ActionItemValue = new
                    {
                        Action = "change_currency",
                        Value = CurrencySelected.CurrCode
                    }
                };
                await _saleEngineService.ActionListItemToOrderAsync(payload);

                await ShoppingCartViewModel.RefreshOrderListAsync();

                RefreshSummary();
            }
            catch { }
        }

        async Task LoadMasterDataAsync()
        {
            try
            {
                await _selfCheckoutService.LoadLanguageAsync();
                await _selfCheckoutService.LoadPaymentAsync();

                Payments = _selfCheckoutService.Payments?.ToObservableCollection();
                PaymentSelected = _selfCheckoutService.Payments.FirstOrDefault();

                Languages = _selfCheckoutService.Languages?.ToObservableCollection();
                LanguageSelected = _selfCheckoutService.Languages.Where(l => l.LangCode == "EN").FirstOrDefault();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
        }

        async Task LoadCurrencyAsync()
        {
            try
            {
                var payload = new
                {
                    branch_no = _selfCheckoutService.AppConfig.BranchNo
                };
                await _saleEngineService.LoadCurrencyAsync(payload);
                Currencies = _saleEngineService.Currencies?.ToObservableCollection();
                SetDefaultCurrency();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
        }

        private void SetDefaultCurrency()
        {
            CurrencySelected = Currencies.Where(c => c.CurrCode == "THB").FirstOrDefault();
        }

        public async Task AddCouponAsync(string couponCode)
        {
            var isWantToUseCoupon = await DialogService.ConfirmAsync(AppResources.ScanCoupon, AppResources.ConfirmUseCoupon, AppResources.Yes, AppResources.No);
            if (!isWantToUseCoupon)
            {
                CouponInputViewModel.CouponCode = "";
                return;
            }

            var payload = new
            {
                OrderGuid = "",
                Rows = new object[] { },
                Action = "add_special_discount_by_qrcode",
                Value = couponCode,
                SessionKey = _saleEngineService.LoginData.SessionKey
            };

            try
            {
                IsBusy = true;
                await _saleEngineService.ActionOrderPaymentAsync(payload);

                CouponCode = _saleEngineService.OrderData.TotalBillingAmount.CurrentValueAdjust?.VaDetail?.Code;

                await ShoppingCartViewModel.RefreshOrderListAsync();
                RefreshSummary();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
                CouponInputViewModel.CouponCode = "";
                CouponInputViewModel.CouponInputViewVisible = false;
            }
        }

        public async Task DeleteCouponAsync()
        {
            var isDeleteCoupon = await DialogService.ConfirmAsync(AppResources.ConfirmDeleteCoupon, CouponCode, AppResources.Yes, AppResources.No, true);
            if (!isDeleteCoupon)
                return;

            var payload = new
            {
                OrderGuid = "",
                Rows = new object[] { },
                Action = "clear_all_special_discount",
                Value = "",
                SessionKey = _saleEngineService.LoginData.SessionKey
            };

            try
            {
                IsBusy = true;
                await _saleEngineService.ActionOrderPaymentAsync(payload);
                CouponCode = "";

                await ShoppingCartViewModel.RefreshOrderListAsync();
                RefreshSummary();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadOrderAsync()
        {
            try
            {
                IsBusy = true;
                var payload = new
                {
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                    Attributes = new object[]
                    {
                        new {
                            GROUP = "tran_no",
                            CODE = "shopping_card",
                            valueOfString = _selfCheckoutService.CurrentShoppingCard
                        }
                    },
                    paging = new
                    {
                        pageNo = 1,
                        pageSize = 100
                    },
                    filter = new object[]
                    {
                        new
                        {
                            sign = "string",
                            element = "order_data",
                            option = "string",
                            type = "string",
                            low = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            height = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        }
                    },
                    sorting = new object[]
                    {
                        new
                        {
                            sortBy = "headerkey",
                            orderBy = "desc"
                        }
                    }
                };

                await _saleEngineService.GetOrderAsync(payload);

                await ShoppingCartViewModel.RefreshOrderListAsync();

                RefreshSummary();
            }
            catch (Exception ex)
            {
                if (ex is KPApiException)
                {
                    if ((ex as KPApiException).ErrorCode == "SESSION_EXPIRE")
                    {
                        await DialogService.ShowAlert(AppResources.Opps, AppResources.CannotConnectToServer, AppResources.Close);
                        await LoginAsync();
                        await LoadOrderAsync();
                    }
                    else
                    {
                        await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
                    }
                }
                else
                {
                    await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task AddOrderAsync(string barcode)
        {
            try
            {
                IsBusy = true;
                var payload = new
                {
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                    ItemCode = barcode
                };
                await _saleEngineService.AddItemToOrderAsync(payload);

                RefreshSummary();

                await ShoppingCartViewModel.RefreshOrderListAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void RefreshSummary()
        {
            OrderData = _saleEngineService.OrderData;
            try
            {
                if (CurrentView is ShoppingCartView)
                {
                    if (OrderData.BillingQty > 0)
                        PageTitle = $"{AppResources.MyCart} ({OrderData.BillingQty})";
                    else
                        PageTitle = AppResources.MyCart;
                }

                var tab = Tabs.Where(t => t.TabId == 3).FirstOrDefault();
                tab.BadgeCount = Convert.ToInt32(OrderData.BillingQty);
            }
            catch { }
        }

        async Task CheckoutAsync()
        {
            try
            {
                IsBusy = true;

                var payload = new
                {
                    OrderGuid = _saleEngineService.OrderData.Guid,
                    SessionKey = _saleEngineService.LoginData.SessionKey
                };

                await _saleEngineService.CheckoutPaymentOrder(payload);

                IsBeingPaymentProcess = true;
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task QRPaymentAsync(PromptPayResult promptPayResult)
        {
            try
            {
                IsBusy = true;

                var paymentPayload = new
                {
                    OrderGuid = _saleEngineService.OrderData.Guid,
                    Payment = new
                    {
                        Guid = "",
                        LineNo = 0,
                        PaymentCode = "PROMT",
                        PaymentType = "VISA",
                        PaymentIcon = "",
                        RefNo = promptPayResult.TransactionId,
                        URLService = "",
                        CardHolderName = "",
                        ApproveCode = "",
                        BankOfEDC = "",
                        IssuerID = "",
                        WalletBarcode = "",
                        WalletMerchantID = "",
                        WalletTransID = "",
                        isDCC = false,
                        isCheckVoucher = false,
                        isFixAmount = false,
                        isNotAllowSMC = false,
                        isComplete = false,
                        PaymentAmounts = new
                        {
                            CurrAmt = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrAmt,
                            CurrCode = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrCode,
                            CurrRate = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrRate,
                            BaseCurrCode = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrCode,
                            BaseCurrRate = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrRate,
                            BaseCurrAmt = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrAmt,
                        },
                        Transaction = new
                        {
                            TransactionId = 0,
                            GatewaySessionKey = "",
                            TransactionGroup = 4,
                            TransactionType = 1,
                            PartnerId = 0,
                            LastStatus = 1,
                            CurrentStatus = 1,
                            Movements = new[]
                            {
                                new
                                {
                                    TransactionMovementType = 1,
                                    Amount = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrAmt,
                                    Currency = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrCode,
                                    Description = "",
                                    Status = 1
                                }
                            }
                        },
                        status = "SUCCESS",
                        PartnerTransID = "",
                        PaymentSessionKey = 1
                    },
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                };

                await ConfirmPaymentAsync(paymentPayload, false);
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Payment, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task WalletPaymentAsync()
        {
            Wallet wallet = null;
            try
            {
                IsBusy = true;
                var walletRequestPayload = new
                {
                    barcode = PaymentBarcode,
                    machine_ip = _saleEngineService.LoginData.UserInfo.MachineEnv.MachineIp,
                    branch_no = _selfCheckoutService.AppConfig.BranchNo
                };

                wallet = await _saleEngineService.GetWalletTypeFromBarcodeAsync(walletRequestPayload);
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert("Get wallet", ex.Message, AppResources.Close);
                PaymentBarcode = "";
            }
            finally
            {
                IsBusy = false;
            }

            if (wallet == null)
                return;

            try
            {
                IsBusy = true;

                var paymentPayload = new
                {
                    OrderGuid = _saleEngineService.OrderData.Guid,
                    Payment = new
                    {
                        Guid = "",
                        LineNo = 0,
                        PaymentCode = PaymentSelected.MethodCode,
                        PaymentType = wallet.WalletType,
                        PaymentIcon = "",
                        RefNo = PaymentBarcode,
                        URLService = wallet.WalletagentMaster.Wsurl,
                        CardHolderName = "",
                        ApproveCode = "",
                        BankOfEDC = "",
                        IssuerID = "",
                        WalletBarcode = PaymentBarcode,
                        WalletMerchantID = wallet.WalletagentMaster.MerchantId,
                        GatewayId = PaymentSelected.GatewayId,
                        WalletTransID = "",
                        isDCC = false,
                        isCheckVoucher = false,
                        isFixAmount = false,
                        isNotAllowSMC = false,
                        isComplete = false,
                        PaymentAmounts = new
                        {
                            CurrAmt = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrAmt,
                            CurrCode = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrCode,
                            CurrRate = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrRate,
                            BaseCurrCode = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrCode,
                            BaseCurrRate = 1,
                            BaseCurrAmt = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrAmt
                        },
                        Transaction = new
                        {
                            TransactionId = 0,
                            GatewaySessionKey = "",
                            TransactionGroup = 2,
                            TransactionType = 1,
                            PartnerId = wallet.WalletagentMaster.PartnertypeId,
                            PartnerType = wallet.WalletagentMaster.PartnertypeId,
                            LastStatus = 1,
                            CurrentStatus = 1,
                            Movements = new[]
                            {
                                    new
                                    {
                                        TransactionMovementType = 1,
                                        Amount = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrAmt,
                                        Description = "",
                                        Status = 1
                                    }
                                }
                        },
                        ChangeAmounts = new
                        {
                            CurrAmt = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrAmt,
                            CurrCode = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrCode,
                            CurrRate = _saleEngineService.OrderData.RemainingAmount.NetAmount.CurrRate,
                            BaseCurrCode = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrCode,
                            BaseCurrRate = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrRate,
                            BaseCurrAmt = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrAmt
                        },
                        status = ""
                    },
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                };

                await ConfirmPaymentAsync(paymentPayload, true);
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Payment, ex.Message, AppResources.Close);
            }
            finally
            {
                PaymentBarcode = "";
                IsBusy = false;
            }
        }

        private void ResetPaymentState()
        {
            SummaryShowing = false;
            IsBeingPaymentProcess = false;
            IsPaymentProcessing = false;
            PaymentInputShowing = false;
            PaymentBarcode = "";
            CouponCode = "";

            SetDefaultCurrency();
        }

        async Task ConfirmPaymentAsync(object paymentPayload, bool isWallet)
        {
            await _saleEngineService.AddPaymentToOrderAsync(paymentPayload);

            var paymentSuccess = false;
            if (isWallet)
            {
                var tokenSource = new CancellationTokenSource();
                var ct = tokenSource.Token;

                IsPaymentProcessing = true;

                PaymentCountdownTimer = _selfCheckoutService.AppConfig.PaymentTimeout;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (paymentSuccess)
                    {
                        return false;
                    }

                    if (--PaymentCountdownTimer == 0)
                    {
                        tokenSource.Cancel();
                        return false;
                    }
                    if (!IsPaymentProcessing)
                        return false;
                    return true;
                });

                while (true)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    var actionPayload = new
                    {
                        OrderGuid = _saleEngineService.OrderData.Guid,
                        Rows = _saleEngineService.OrderData.OrderPayments?.Select(p => p.Guid).ToArray(),
                        Action = 3,
                        Value = "",
                        currency = "",
                        SessionKey = _saleEngineService.LoginData.SessionKey
                    };
                    var paymentStatus = await _saleEngineService.ActionPaymentToOrderAsync(actionPayload);
                    if (paymentStatus.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        paymentSuccess = true;
                        break;
                    }
                }
            }
            else
            {
                paymentSuccess = true;
            }

            if (paymentSuccess)
            {
                var finishPaymentPayload = new
                {
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                    OrderGuid = _saleEngineService.OrderData.Guid,
                    OrderSignature = new object[]
                    {
                            new
                            {
                                code = "",
                                signature = ""
                            }
                    }
                };
                await _saleEngineService.FinishPaymentOrderAsync(finishPaymentPayload);

                var headerAttr = _saleEngineService.OrderData.HeaderAttributes.Where(o => o.Code == "order_no").FirstOrDefault();
                var orderNo = Convert.ToInt32(headerAttr.ValueOfDecimal);
                var result = await _selfCheckoutService.UpdateSessionAsync(_selfCheckoutService.BorrowSessionKey, orderNo, _selfCheckoutService.CurrentShoppingCard);

                ResetPaymentState();
                await LoginAsync();

                var shoppingCartTab = Tabs.Where(t => t.TabId == 3).FirstOrDefault();
                await LoadOrderAsync();

                var isContinueShopping = await DialogService.ConfirmAsync(AppResources.ThkForOrderTitle, AppResources.ThkForOrderDetail, AppResources.ContinueShopping, AppResources.MyOrder);
                if (!isContinueShopping)
                {
                    var orderTab = Tabs.Where(t => t.TabId == 4).FirstOrDefault();
                    TabSelectedCommand.Execute(orderTab);
                }
            }
        }

        private async Task LoginAsync()
        {
            var appConfig = _selfCheckoutService.AppConfig;
            var loginData = await _saleEngineService.LoginAsync(appConfig.UserName, appConfig.Password);
            _saleEngineService.LoginData = loginData;
        }
    }
}
