﻿using Newtonsoft.Json;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using SelfCheckout.Exceptions;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services;
using SelfCheckout.Services.Payment;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using SelfCheckout.Views;
using System;
using System.Collections.Generic;
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
        IPaymentService _paymentService;
        IRegisterService _registerService;

        ObservableCollection<TabItem> _tabs;
        ObservableCollection<Currency> _currencies;
        ObservableCollection<Payment> _payments;

        ContentView _currentView;
        Currency _currencySelected;
        Payment _paymentSelected;
        OrderData _orderData;

        int _selectedTabId;
        int _paymentCountdownTimer;

        string _paymentBarCode;
        string _couponCode;
        string _checkoutButtonText;

        bool _headerLogoVisible;
        bool _systemViewVisible;
        bool _summaryVisible;
        bool _summaryShowing;
        bool _currencyShowing;
        bool _isBeingPaymentProcess;
        bool _isPaymentProcessing;
        bool _paymentSelectionShowing;
        bool _paymentInputShowing;
        bool _isChangingShoppingCard;

        bool _isPromptPayProcessing;
        bool _isCameraScannerOpening;

        public MainViewModel(INavigationService navigationService,
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService,
            IRegisterService registerService, IPaymentService paymentService) : base(navigationService)
        {
            _saleEngineService = saleEngineService;
            _selfCheckoutService = selfCheckoutService;
            _paymentService = paymentService;
            _registerService = registerService;

            HomeViewModel = new HomeViewModel(navigationService, selfCheckoutService);
            HomeViewModel.ShowSystemView = () => SystemViewVisible = true;

            DeviceViewModel = new DeviceViewModel(saleEngineService, registerService);

            ShoppingCartViewModel = new ShoppingCartViewModel(navigationService, selfCheckoutService, saleEngineService, registerService);
            ShoppingCartViewModel.ReloadOrderDataAsync = LoadOrderAsync;
            ShoppingCartViewModel.ChangeCurrencyAsync = ChangeCurrency;
            ShoppingCartViewModel.RefreshSummary = RefreshSummary;
            ShoppingCartViewModel.ShoppingCardChanging = (isChanging) => _isChangingShoppingCard = isChanging;

            OrderViewModel = new OrderViewModel(navigationService, selfCheckoutService, saleEngineService, registerService);
            OrderViewModel.ShowOrHideLoading = (isBusy) => IsBusy = isBusy;

            ProfileViewModel = new ProfileViewModel(selfCheckoutService);

            TutorialViewModel = new TutorialViewModel(navigationService, selfCheckoutService);

            CouponInputViewModel = new CouponInputViewModel(navigationService);
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

            RefreshTab();

            await LoadMasterDataAsync();
            await LoadCurrencyAsync();
        }

        public ICommand TabSelectedCommand => new Command<TabItem>(async (item) => await SelectTabAsync(item));

        public ICommand ScanCommand => new Command<object>(async (data) =>
        {
            if (PaymentInputShowing && IsBeingPaymentProcess)
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
                    {
                        if (!string.IsNullOrEmpty(_saleEngineService.OrderData.TotalBillingAmount.CurrentValueAdjust?.VaDetail?.Code))
                            return;
                        await AddOrderAsync(data?.ToString());
                    }
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

        public ICommand ShowInfoCommand => new Command(async () =>
        {
            await TutorialViewModel.ShowTutorialAsync();
        });

        public ICommand CurrencyTappedCommand => new Command(() =>
        {
            CurrencyShowing = !CurrencyShowing;
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
                var result = await NavigationService.ConfirmAsync(AppResources.Logout, AppResources.ConfirmLogout, AppResources.Yes, AppResources.No);
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

        public ICommand ScanPaymentCommand => new Command<object>(async (type) =>
            {
                if (IsPaymentProcessing)
                    return;

                if (Convert.ToInt32(type) == 1)
                {
                    lock (_lockProcess)
                    {
                        if (_isCameraScannerOpening)
                            return;
                        else
                            _isCameraScannerOpening = true;
                    }
                    var result = await NavigationService.ShowDialogAsync<string>("CameraScannerView", null);
                    _isCameraScannerOpening = false;

                    if (!string.IsNullOrEmpty(result))
                    {
                        PaymentBarcode = result;
                        await WalletPaymentAsync();
                    }
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
            bool isShoppingCartEmpty = true;
            try
            {
                if (_saleEngineService.OrderData.OrderDetails.Any())
                    isShoppingCartEmpty = false;
            }
            catch { }

            if (isShoppingCartEmpty)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, AppResources.ShoppingCartEmpty);
                return;
            }

            if (!IsBeingPaymentProcess)
            {
                await CheckoutAsync();
            }
            else
            {
                if (!PaymentSelected.IsAlipay)
                {
                    lock (_lockProcess)
                    {
                        if (_isPromptPayProcessing)
                            return;
                        else
                            _isPromptPayProcessing = true;
                    }

                    // try inquiry from noti first
                    PromptPayResult promptPayResult = null;
                    try
                    {
                        IsBusy = true;
                        var refNo = _paymentService.GetPaymentRefNo();
                        await _paymentService.InquiryAsync(refNo);
                    }
                    catch { }
                    finally
                    {
                        IsBusy = false;
                    }
                    if (promptPayResult == null)
                    {
                        var result = await NavigationService.ShowDialogAsync<INavigationParameters>("PromptPayQrDialog", null);
                        promptPayResult = result.GetValue<PromptPayResult>("PromptPayResult");
                    }
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
                    _isPromptPayProcessing = false;
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

                if (value is HomeView)
                {
                    HeaderLogoVisible = true;
                }
                else
                {
                    HeaderLogoVisible = false;
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

        public bool HeaderLogoVisible
        {
            get => _headerLogoVisible;
            set => SetProperty(ref _headerLogoVisible, value);
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

        async Task SelectTabAsync(TabItem item)
        {
            var isSessionAlreadyEnd = false;
            if (!(item.Page is OrderView) && !(item.Page is ShoppingCartView))
            {
                isSessionAlreadyEnd = await CheckSessionAlreadyEndAsync();
            }

            if (!isSessionAlreadyEnd)
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
            }
        }

        async Task<bool> CheckSessionAlreadyEndAsync()
        {
            try
            {
                IsBusy = true;
                var sessionData = await _selfCheckoutService.GetSessionDetialAsync(_selfCheckoutService.BorrowSessionKey.ToString());
                if (sessionData.SessionStatus.SessionCode == "END")
                {
                    await _saleEngineService.LogoutAsync();
                    await GoBackToRootAsync();
                    return true;
                }
            }
            catch { }
            finally
            {
                IsBusy = false;
            }
            return false;
        }

        protected override async Task OnLanguageChanged(Language lang)
        {
            try
            {
                foreach (var tab in Tabs)
                {
                    switch (tab.TabId)
                    {
                        case 1:
                            tab.TabText = AppResources.Home;
                            tab.Title = AppResources.Home;
                            break;
                        case 2:
                            tab.TabText = AppResources.Device;
                            tab.Title = AppResources.DeviceInfo;
                            break;
                        case 3:
                            tab.TabText = AppResources.Shopping;
                            break;
                        case 4:
                            tab.TabText = AppResources.Orders;
                            tab.Title = AppResources.Orders;
                            break;
                        case 5:
                            tab.TabText = AppResources.Profile;
                            tab.Title = AppResources.Profile;
                            break;
                    }

                    if (tab.TabId == _selectedTabId)
                    {
                        PageTitle = tab.Title;
                    }
                }
            }
            catch { }

            MessagingCenter.Send(this, "LanguageChange");

            await Task.Run(async () =>
            {
                if (CurrentView is ShoppingCartView)
                {
                    await ShoppingCartViewModel.RefreshOrderListAsync();
                    RefreshSummary();
                }
                else if (CurrentView is OrderView)
                {
                    await OrderViewModel.RefreshOrderAsync();
                }

                await HomeViewModel.ReloadImageAsset();
                TutorialViewModel.RefreshLanguage();
                DeviceViewModel.RefreshLanguage();
                ShoppingCartViewModel.RefreshLanguage();
                OrderViewModel.RefreshLanguage();
            });
        }

        protected override Task OnLanguageViewShowingChanged(bool isShowing)
        {
            if (isShowing)
                CurrencyShowing = false;
            return Task.FromResult(true);
        }

        async Task ChangeCurrency()
        {
            if (CurrentView is ShoppingCartView)
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
            else if (CurrentView is OrderView)
            {
                await OrderViewModel.RefreshOrderAsync();
            }
        }

        async Task LoadMasterDataAsync()
        {
            try
            {
                Payments = _selfCheckoutService.Payments?.ToObservableCollection();
                PaymentSelected = _selfCheckoutService.Payments.FirstOrDefault();

                Languages = _selfCheckoutService.Languages?.ToObservableCollection();
                LanguageSelected = _selfCheckoutService.CurrentLanguage;
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message);
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
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message);
            }
        }

        private void SetDefaultCurrency()
        {
            CurrencySelected = Currencies.Where(c => c.CurrCode == "THB").FirstOrDefault();
        }

        public async Task AddCouponAsync(string couponCode)
        {
            var isWantToUseCoupon = await NavigationService.ConfirmAsync(AppResources.ScanCoupon, AppResources.ConfirmUseCoupon, AppResources.Yes, AppResources.No);
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

                CouponCode = _saleEngineService.CouponCode;

                await ShoppingCartViewModel.RefreshOrderListAsync();
                RefreshSummary();
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
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
            var isDeleteCoupon = await NavigationService.ConfirmAsync(AppResources.ConfirmDeleteCoupon, CouponCode, AppResources.Yes, AppResources.No, true);
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

                var filter = new string[] { "ALI", "WEC", "PMP" };
                var pendingPayment = _saleEngineService.OrderData.OrderPayments.Where(p => filter.Contains(p.PaymentCode)).FirstOrDefault();
                if (pendingPayment != null)
                {
                    Xamarin.Forms.DependencyService.Get<ILogService>()?.LogInfo("Cancel payment after delete promotion");
                    await CancelPaymentAsync();
                }

                await _saleEngineService.ActionOrderPaymentAsync(payload);
                CouponCode = "";

                await ShoppingCartViewModel.RefreshOrderListAsync();
                RefreshSummary();
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
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
                var attrs = new List<object>
                {
                    new {
                        GROUP = "tran_no",
                        CODE = "shopping_card",
                        valueOfString = _selfCheckoutService.CurrentShoppingCard
                    }
                };

                if (_registerService.CustomerData?.IsMember == true)
                {
                    var identities = _registerService.CustomerData.Person?.ListIdentity;
                    var memberId = identities?.Where(i => i.IdentityType == "MID").FirstOrDefault()?.IdentityValue;
                    var cardGroupCode = identities?.Where(i => i.IdentityType == "CARDGROUPCODE").FirstOrDefault()?.IdentityValue;
                    var cardTypeCode = identities?.Where(i => i.IdentityType == "CARDTYPECODE").FirstOrDefault()?.IdentityValue;
                    var embossId = identities?.Where(i => i.IdentityType == "EMBOSSID").FirstOrDefault()?.IdentityValue;
                    attrs.Add(new
                    {
                        GROUP = "tran_no",
                        CODE = "member_id",
                        valueOfString = memberId
                    });
                    attrs.Add(new
                    {
                        GROUP = "tran_no",
                        CODE = "CARDGROUPCODE",
                        valueOfString = cardGroupCode
                    });
                    attrs.Add(new
                    {
                        GROUP = "tran_no",
                        CODE = "CARDTYPECODE",
                        valueOfString = cardTypeCode
                    });
                    attrs.Add(new
                    {
                        GROUP = "tran_no",
                        CODE = "EMBOSSID",
                        valueOfString = embossId
                    });
                }

                var payload = new
                {
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                    Attributes = attrs.ToArray(),
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
                            element = "order_date",
                            option = "string",
                            type = "string",
                            low = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            high = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
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
                    var apiException = ex as KPApiException;
                    if (apiException.ErrorCode.Equals("SESSION_EXPIRE", StringComparison.OrdinalIgnoreCase))
                    {
                        await NavigationService.ShowAlertAsync(AppResources.Opps, AppResources.CannotConnectToServer, AppResources.Close);
                        await LoginAsync();
                        await LoadOrderAsync();
                    }
                    else if (apiException.ErrorCode.Equals("EX1", StringComparison.OrdinalIgnoreCase))
                    {
                        await LoginAsync();
                        await LoadOrderAsync();
                    }
                    else
                    {
                        await NavigationService.ShowAlertAsync(AppResources.Opps, $"{apiException.ErrorCode} {apiException.Message}", AppResources.Close);
                    }
                }
                else
                {
                    await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
                }
                Xamarin.Forms.DependencyService.Get<ILogService>().LogError(ex.Message, ex);
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
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message);
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

                OrderPayment inquiryResult = null;
                try
                {
                    Xamarin.Forms.DependencyService.Get<ILogService>().LogInfo("Checkout Inquiry");
                    inquiryResult = await InquirePaymentAsync();
                }
                catch { }

                bool canCheckout = false;
                if (inquiryResult != null)
                {
                    try
                    {
                        //if (inquiryResult.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    await FinishPaymentAsync();
                        //}
                        //else
                        //{
                        await CancelPaymentAsync();
                        canCheckout = true;
                        //}
                    }
                    catch (Exception ex)
                    {
                        await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
                    }
                }
                else
                {
                    canCheckout = true;
                }

                if (canCheckout)
                {
                    var payload = new
                    {
                        OrderGuid = _saleEngineService.OrderData.Guid,
                        SessionKey = _saleEngineService.LoginData.SessionKey
                    };

                    await _saleEngineService.CheckoutPaymentOrder(payload);

                    IsBeingPaymentProcess = true;
                }
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
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
                        RefNo = promptPayResult.BillPaymentRef1,
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
                await NavigationService.ShowAlertAsync(AppResources.Payment, ex.Message, AppResources.Close);
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

                if (!wallet.WalletType.Equals(PaymentSelected.MethodDesc, StringComparison.OrdinalIgnoreCase))
                {
                    await NavigationService.ShowAlertAsync(AppResources.Payment, AppResources.PaymentTypeNotMatch, AppResources.Close);
                    PaymentBarcode = "";
                    return;
                }
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync("Get wallet", ex.Message, AppResources.Close);
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
                await NavigationService.ShowAlertAsync(AppResources.Payment, ex.Message, AppResources.Close);
                ResetPaymentState(false);
            }
            finally
            {
                PaymentBarcode = "";
                IsBusy = false;
            }
        }

        private void ResetPaymentState(bool isPaymentSuccess)
        {
            if (isPaymentSuccess)
            {
                SummaryShowing = false;
                CouponCode = "";
            }
            IsBeingPaymentProcess = false;
            IsPaymentProcessing = false;
            PaymentInputShowing = false;
            PaymentBarcode = "";
        }

        async Task ConfirmPaymentAsync(object paymentPayload, bool isWallet)
        {
            Xamarin.Forms.DependencyService.Get<ILogService>().LogInfo("Add Payment");

            await _saleEngineService.AddPaymentToOrderAsync(paymentPayload);
            IsBusy = false;

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

                var tryCounterInCaseFail = 0;
                var tryCounterInCaseTimeout = 0;
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        if (tryCounterInCaseTimeout++ == 2)
                            break;
                    }

                    if (!ct.IsCancellationRequested)
                        await Task.Delay(3000);
                    else
                        await Task.Delay(1000);

                    var paymentStatus = await InquirePaymentAsync();
                    if (paymentStatus.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        paymentSuccess = true;
                        break;
                    }
                    else if (paymentStatus.Status.Equals("FAIL", StringComparison.OrdinalIgnoreCase))
                    {
                        if (++tryCounterInCaseFail == 2)
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
                await FinishPaymentAsync();
            }
            else
            {
                ResetPaymentState(paymentSuccess);
            }
        }

        async Task<OrderPayment> CancelPaymentAsync()
        {
            Xamarin.Forms.DependencyService.Get<ILogService>().LogInfo("Cancel Payment");
            var actionPayload = new
            {
                OrderGuid = _saleEngineService.OrderData.Guid,
                Rows = new string[] { },
                Action = 2,
                Value = "",
                currency = "",
                SessionKey = _saleEngineService.LoginData.SessionKey
            };
            return await _saleEngineService.ActionPaymentToOrderAsync(actionPayload);
        }

        async Task<OrderPayment> InquirePaymentAsync()
        {
            var paymentGuid = _saleEngineService.OrderData.OrderPayments?.Where(p => new string[] { "ALI", "WEC", "PMP" }.Contains(p.PaymentCode)).Select(p => p.Guid).ToArray();
            var actionPayload = new
            {
                OrderGuid = _saleEngineService.OrderData.Guid,
                Rows = paymentGuid,
                Action = 3,
                Value = "",
                currency = "",
                SessionKey = _saleEngineService.LoginData.SessionKey
            };
            return await _saleEngineService.ActionPaymentToOrderAsync(actionPayload);
        }

        async Task FinishPaymentAsync()
        {
            Xamarin.Forms.DependencyService.Get<ILogService>().LogInfo("Finish Payment");

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

            ResetPaymentState(true);
            await LoginAsync();

            var isContinueShopping = await NavigationService.ConfirmAsync(AppResources.ThkForOrderTitle, AppResources.ThkForOrderDetail, AppResources.ContinueShopping, AppResources.MyOrder);
            if (isContinueShopping)
            {
                await LoadOrderAsync();
            }
            else
            {
                // cause by KP-SCO-0089
                // he don't want to create record df_sohdr, so i have to use this way because i can't call function api/SaleEngine/GetOrder
                _saleEngineService.OrderData = new OrderData()
                {
                    OrderDetails = new System.Collections.Generic.List<OrderDetail>(),
                    BillingQuantities = new System.Collections.Generic.List<BillingQuantity>()
                        {
                            new BillingQuantity()
                            {
                                Quantity = 0
                            },
                            new BillingQuantity()
                            {
                                Quantity = 0
                            }
                        }
                };
                await ShoppingCartViewModel.RefreshOrderListAsync();
                RefreshSummary();

                var orderTab = Tabs.Where(t => t.TabId == 4).FirstOrDefault();
                TabSelectedCommand.Execute(orderTab);
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
