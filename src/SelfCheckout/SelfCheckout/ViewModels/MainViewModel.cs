﻿using Newtonsoft.Json;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Controls;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Base;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using SelfCheckout.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
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

        int _paymentCountdownTimer;

        string _paymentBarCode;
        string _couponCode;
        string _checkoutButtonText;

        bool _summaryVisible;
        bool _summaryShowing;
        bool _langShowing;
        bool _currencyShowing;
        bool _isBeingPaymentProcess;
        bool _isPaymentProcessing;
        bool _paymentSelectionShowing;
        bool _paymentInputShowing;

        public MainViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService) : base(navigatinService, dialogService)
        {
            _saleEngineService = saleEngineService;
            _selfCheckoutService = selfCheckoutService;

            MessagingCenter.Subscribe<DeviceViewModel>(this, "Logout", async (s) =>
            {
                await NavigationService.GoBackToRootAsync();
            });

            MessagingCenter.Subscribe<ShoppingCartViewModel, OrderDetail>(this, "ShowOrderDetail", async (sender, order) =>
            {
                await NavigationService.NavigateAsync("OrderDetailView", new NavigationParameters() { { "OrderDetail", order } });
            });

            MessagingCenter.Subscribe<ShoppingCartViewModel>(this, "OrderRefresh", (s) =>
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
            });
        }

        private void RefreshTab()
        {
            try
            {
                if (Tabs.Any())
                {
                    foreach (var tab in Tabs)
                    {
                        (tab.Page.BindingContext as ViewModelBase).Destroy();
                    }
                }
            }
            catch { }

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
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await LoadMasterDataAsync();
            await LoadCurrencyAsync();
        }

        public override void Destroy()
        {
            MessagingCenter.Unsubscribe<DeviceViewModel>(this, "Logout");
            MessagingCenter.Unsubscribe<ViewModelBase>(this, "OrderRefresh");
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
                MessagingCenter.Send(this, "AddItemToOrder", data?.ToString());
            }
        });

        public ICommand ScanCouponCommand => new Command(async () =>
        {
            if (!string.IsNullOrEmpty(CouponCode))
            {
                var isDeleteCoupon = await DialogService.ConfirmAsync(AppResources.Delete, AppResources.ConfirmDeleteCoupon, AppResources.Yes, AppResources.No, true);
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
                    await _saleEngineService.ActionOrderPaymentAsync(payload);
                    CouponCode = "";
                    MessagingCenter.Send(this, "OrderRefresh");
                }
                catch (Exception ex)
                {
                    await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
                }
            }
            else
            {
                var isWantToUseCoupon = await DialogService.ConfirmAsync(AppResources.ScanCoupon, AppResources.ConfirmUseCoupon, AppResources.Yes, AppResources.No);
                if (!isWantToUseCoupon)
                    return;
                DialogService.ShowDialog("BarcodeScanDialog", null, async (scanResult) =>
                {
                    var result = scanResult.Parameters.GetValue<string>("ScanData");
                    if (!string.IsNullOrEmpty(result))
                    {
                        var payload = new
                        {
                            OrderGuid = "",
                            Rows = new object[] { },
                            Action = "add_special_discount_by_qrcode",
                            Value = result,
                            SessionKey = _saleEngineService.LoginData.SessionKey
                        };

                        try
                        {
                            await _saleEngineService.ActionOrderPaymentAsync(payload);

                            CouponCode = _saleEngineService.OrderData.TotalBillingAmount.CurrentValueAdjust?.VaDetail?.Code;

                            MessagingCenter.Send(this, "OrderRefresh");
                        }
                        catch (Exception ex)
                        {
                            await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
                        }
                    }
                });
            }
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

        public ICommand PaymentMethodTappedCommand => new Command(() =>
        {
            PaymentSelectionShowing = !PaymentSelectionShowing;
        });

        public ICommand PaymentSelectionCommand => new Command<Payment>(async (payment) =>
        {
            PaymentSelectionShowing = false;
            PaymentSelected = payment;

            if (!payment.IsAlipay)
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
                PaymentInputShowing = true;
            }
        });

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
            set => SetProperty(ref _currentView, value);
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
            set => SetProperty(ref _languageSelected, value, () =>
            {
                _selfCheckoutService.CurrentLanguage = value;

                if (value.LangCode == "EN")
                    GlobalSettings.Instance.CountryCode = "en-US";
                else if (value.LangCode == "TH")
                    GlobalSettings.Instance.CountryCode = "th-TH";
                else if (value.LangCode == "CH")
                    GlobalSettings.Instance.CountryCode = "zh-Hans";
                GlobalSettings.Instance.InitLanguage();

                MessagingCenter.Send(this, "LanguageChanged");
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

        Task SelectTabAsync(TabItem item)
        {
            if (item.Page is ShoppingCartView)
            {
                SummaryVisible = true;
            }
            else
            {
                SummaryVisible = false;

                if (item.Page is OrderView)
                {
                    IsBeingPaymentProcess = false;
                }
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
                MessagingCenter.Send(this, "CurrencyChanged");
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
                CurrencySelected = Currencies.Where(c => c.CurrCode == "THB").FirstOrDefault();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message);
            }
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

                var netAmount = _saleEngineService.OrderData.BillingAmount.NetAmount.CurrAmt;
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
                            CurrAmt = netAmount,
                            CurrCode = new
                            {
                                Code = _saleEngineService.CurrencySelected.CurrCode,
                                Desc = _saleEngineService.CurrencySelected.CurrDesc
                            },
                            CurrRate = _saleEngineService.CurrencySelected.CurrRate,
                            BaseCurrCode = new
                            {
                                Code = _saleEngineService.BaseCurrency.CurrCode,
                                Desc = _saleEngineService.BaseCurrency.CurrDesc
                            },
                            BaseCurrRate = 1,
                            BaseCurrAmt = netAmount
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
                                        Amount = netAmount,
                                        Currency = new
                                        {
                                            Code = _saleEngineService.BaseCurrency.CurrCode,
                                            Desc = _saleEngineService.BaseCurrency.CurrDesc
                                        },
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

                await ConfirmPaymentAsync(paymentPayload);
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
                IsBeingPaymentProcess = false;
                IsPaymentProcessing = false;
                PaymentInputShowing = false;
            }
        }

        async Task WalletPaymentAsync()
        {
            try
            {
                IsBusy = true;
                var walletRequestPayload = new
                {
                    barcode = PaymentBarcode,
                    machine_ip = _saleEngineService.LoginData.UserInfo.MachineEnv.MachineIp,
                    branch_no = _selfCheckoutService.AppConfig.BranchNo
                };
                var wallet = await _saleEngineService.GetWalletTypeFromBarcodeAsync(walletRequestPayload);

                var netAmount = _saleEngineService.OrderData.BillingAmount.NetAmount.CurrAmt;
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
                            CurrAmt = netAmount,
                            CurrCode = new
                            {
                                Code = _saleEngineService.CurrencySelected.CurrCode,
                                Desc = _saleEngineService.CurrencySelected.CurrDesc
                            },
                            CurrRate = _saleEngineService.CurrencySelected.CurrRate,
                            BaseCurrCode = new
                            {
                                Code = _saleEngineService.BaseCurrency.CurrCode,
                                Desc = _saleEngineService.BaseCurrency.CurrDesc
                            },
                            BaseCurrRate = 1,
                            BaseCurrAmt = netAmount
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
                                        Amount = netAmount,
                                        Description = "",
                                        Status = 1
                                    }
                                }
                        },
                        ChangeAmounts = new
                        {
                            CurrAmt = netAmount,
                            CurrCode = new
                            {
                                Code = _saleEngineService.CurrencySelected.CurrCode,
                                Desc = _saleEngineService.CurrencySelected.CurrDesc
                            },
                            CurrRate = _saleEngineService.CurrencySelected.CurrRate,
                            BaseCurrCode = new
                            {
                                Code = _saleEngineService.BaseCurrency.CurrCode,
                                Desc = _saleEngineService.BaseCurrency.CurrDesc
                            },
                            BaseCurrRate = 1,
                            BaseCurrAmt = netAmount
                        },
                        status = ""
                    },
                    SessionKey = _saleEngineService.LoginData.SessionKey,
                };

                await ConfirmPaymentAsync(paymentPayload);

                try
                {
                    // Test void payment
                    await _saleEngineService.VoidPaymentAsync(wallet.WalletagentMaster.MerchantId, _saleEngineService.OrderData.PaymentTransaction.PartnerTransId);
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
                IsBeingPaymentProcess = false;
                IsPaymentProcessing = false;
                PaymentInputShowing = false;
                PaymentBarcode = "";
            }
        }

        async Task ConfirmPaymentAsync(object paymentPayload)
        {
            await _saleEngineService.AddPaymentToOrderAsync(paymentPayload);

            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;

            PaymentCountdownTimer = _selfCheckoutService.AppConfig.PaymentTimeout;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (--PaymentCountdownTimer == 0)
                {
                    tokenSource.Cancel();
                    return false;
                }
                if (!IsPaymentProcessing)
                    return false;
                return true;
            });

            IsPaymentProcessing = true;
            var paymentSuccess = false;
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
                    IsBeingPaymentProcess = false;
                    IsPaymentProcessing = false;
                    paymentSuccess = true;
                    break;
                }
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

                var appConfig = _selfCheckoutService.AppConfig;
                var loginData = await _saleEngineService.LoginAsync(appConfig.UserName, appConfig.Password);
                _saleEngineService.LoginData = loginData;

                var shoppingCartTab = Tabs.Where(t => t.TabId == 3).FirstOrDefault();
                await (shoppingCartTab.Page.BindingContext as ShoppingCartViewModel).LoadOrderAsync();

                var isContinueShopping = await DialogService.ConfirmAsync(AppResources.ThkForOrderTitle, AppResources.ThkForOrderDetail, AppResources.ContinueShopping, AppResources.MyOrder);
                if (!isContinueShopping)
                {
                    var orderTab = Tabs.Where(t => t.TabId == 4).FirstOrDefault();
                    TabSelectedCommand.Execute(orderTab);
                }
            }
        }
    }
}
