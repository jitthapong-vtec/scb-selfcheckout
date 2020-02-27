using SelfCheckout.Controls;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Base;
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

        int _paymentCountdownTimer;

        bool _langShowing;
        bool _currencyShowing;
        bool _summaryVisible;
        bool _summaryShowing;
        bool _isSummaryOrder;
        bool _isBeingPaymentProcess;
        bool _isPaymentProcessing;
        bool _paymentSelectionShowing;
        bool _paymentInputShowing;

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
            if (IsBeingPaymentProcess)
            {
                PaymentBarcode = data?.ToString();
                if (!string.IsNullOrEmpty(PaymentBarcode))
                    await PaymentAsync();
            }
            else
            {
                try
                {
                    var tab = Tabs.Where(t => t.TabId == 3).FirstOrDefault();
                    await (tab.Page.BindingContext as ShoppingCartViewModel).AddOrderAsync(data?.ToString());
                }
                catch { }
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

        public ICommand CurrencySelectionCommand => new Command<Currency>(async (currency) =>
        {
            CurrencySelected = currency;
            CurrencyShowing = false;

            await ChangeCurrency();
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
            if (!IsBeingPaymentProcess)
            {
                await CheckoutAsync();
            }
            else
            {
                PaymentInputShowing = true;
            }
        });

        Task SelectTabAsync(TabItem item)
        {
            if (item.Page is ShoppingCartView || item.Page is OrderView)
            {
                SummaryVisible = true;
                if (item.Page is OrderView)
                {
                    IsBeingPaymentProcess = false;
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

        bool PaymentCountdown()
        {
            return false;
        }

        async Task ChangeCurrency()
        {
            try
            {
                var payload = new
                {
                    SessionKey = LoginData.SessionKey,
                    ActionItemValue = new
                    {
                        Action = "change_currency",
                        Value = CurrencySelected.CurrCode
                    }
                };
                await SaleEngineService.ActionListItemToOrderAsync(payload);
                await (CurrentView.BindingContext as ShoppingCartViewModel).RefreshOrderAsync();
            }
            catch { }
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
                CurrencySelected = SaleEngineService.Currencies.Where(c => c.CurrCode == "THB").FirstOrDefault();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message);
            }
        }

        async Task CheckoutAsync()
        {
            try
            {
                IsBusy = true;

                var payload = new
                {
                    OrderGuid = OrderData.Guid,
                    SessionKey = LoginData.SessionKey
                };

                await SaleEngineService.CheckoutPaymentOrder(payload);

                IsBeingPaymentProcess = true;
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
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
                if (!walletResult.IsCompleted)
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, walletResult.DefaultMessage, AppResources.Close);
                }

                var wallet = walletResult.Data;
                var netAmount = OrderData.BillingAmount.NetAmount.CurrAmt;
                var paymentRequestPayload = new
                {
                    OrderGuid = OrderData.Guid,
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
                                Code = CurrencySelected.CurrCode,
                                Desc = CurrencySelected.CurrDesc
                            },
                            CurrRate = CurrencySelected.CurrRate,
                            BaseCurrCode = new
                            {
                                Code = BaseCurrency.CurrCode,
                                Desc = BaseCurrency.CurrDesc
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
                                Code = CurrencySelected.CurrCode,
                                Desc = CurrencySelected.CurrDesc
                            },
                            CurrRate = CurrencySelected.CurrRate,
                            BaseCurrCode = new
                            {
                                Code = BaseCurrency.CurrCode,
                                Desc = BaseCurrency.CurrDesc
                            },
                            BaseCurrRate = 1,
                            BaseCurrAmt = netAmount
                        },
                        status = ""
                    },
                    SessionKey = LoginData.SessionKey,
                };

                await SaleEngineService.AddPaymentToOrderAsync(paymentRequestPayload);

                var tokenSource = new CancellationTokenSource();
                var ct = tokenSource.Token;

                PaymentCountdownTimer = 10;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (--PaymentCountdownTimer == 0)
                    {
                        tokenSource.Cancel();
                        return false;
                    }
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
                        OrderGuid = OrderData.Guid,
                        Rows = SaleEngineService.OrderData.OrderPayments?.Select(p => p.Guid).ToArray(),
                        Action = 3,
                        Value = "",
                        currency = "",
                        SessionKey = LoginData.SessionKey
                    };
                    var paymentStatus = await SaleEngineService.ActionPaymentToOrderAsync(actionPayload);
                    if (paymentStatus.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        paymentSuccess = true;
                        break;
                    }
                }

                if (paymentSuccess)
                {
                    var finishPaymentPayload = new
                    {
                        SessionKey = LoginData.SessionKey,
                        OrderGuid = OrderData.Guid,
                        OrderSignature = new object[]
                        {
                            new
                            {
                                code = "",
                                signature = ""
                            }
                        }
                    };

                    await SaleEngineService.FinishPaymentOrderAsync(finishPaymentPayload);
                    await DialogService.ShowAlertAsync(AppResources.ThkForOrderTitle, AppResources.ThkForOrderDetail, AppResources.Close);
                    var orderTab = Tabs.Where(t => t.TabId == 4).FirstOrDefault();
                    TabSelectedCommand.Execute(orderTab);
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
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

        public int PaymentCountdownTimer
        {
            get => _paymentCountdownTimer;
            set
            {
                _paymentCountdownTimer = value;
                RaisePropertyChanged(() => PaymentCountdownTimer);
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

        public bool IsPaymentProcessing
        {
            get => _isPaymentProcessing;
            set
            {
                _isPaymentProcessing = value;
                RaisePropertyChanged(() => IsPaymentProcessing);
            }
        }

        public bool IsBeingPaymentProcess
        {
            get => _isBeingPaymentProcess;
            set
            {
                _isBeingPaymentProcess = value;
                RaisePropertyChanged(() => IsBeingPaymentProcess);
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

        public bool PaymentInputShowing
        {
            get => _paymentInputShowing;
            set
            {
                _paymentInputShowing = value;
                RaisePropertyChanged(() => PaymentInputShowing);
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
