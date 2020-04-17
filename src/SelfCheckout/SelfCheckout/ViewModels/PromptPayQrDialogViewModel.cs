using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Payment;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class PromptPayQrDialogViewModel : PopupNavigationBase<INavigationParameters>
    {
        CancellationTokenSource _tokenSource;
        CancellationToken _ct;

        IPaymentService _paymentService;
        ISaleEngineService _saleEngineService;

        string _refNo;
        string _qrData;
        string _msg;
        int _countDown;
        bool _isCountdownStarted;
        bool _isBusy;
        bool _isQrVisible;
        bool _isCloseBtnVisible;

        public PromptPayQrDialogViewModel(INavigationService navigationService, IPaymentService paymentService,
            ISaleEngineService saleEngineService) : base(navigationService)
        {
            _paymentService = paymentService;
            _saleEngineService = saleEngineService;

            _tokenSource = new CancellationTokenSource();
            _ct = _tokenSource.Token;

            _refNo = GetRefNo();
        }

        public ICommand CancelCommand => new DelegateCommand(async () =>
        {
            await SetResult(null);
        });

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public int CountDown
        {
            get => _countDown;
            set => SetProperty(ref _countDown, value);
        }

        public bool IsCountdownStarted
        {
            get => _isCountdownStarted;
            set => SetProperty(ref _isCountdownStarted, value);
        }

        public bool IsQrVisible
        {
            get => _isQrVisible;
            set => SetProperty(ref _isQrVisible, value);
        }

        public string Msg
        {
            get => _msg;
            set => SetProperty(ref _msg, value);
        }

        public bool IsCloseBtnVisible
        {
            get => _isCloseBtnVisible;
            set => SetProperty(ref _isCloseBtnVisible, value);
        }

        public string QRData
        {
            get => _qrData;
            set => SetProperty(ref _qrData, value);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            IsCloseBtnVisible = true;

            var canGenQr = await CreateQRCodeAsync();
            if (canGenQr)
            {
                IsCloseBtnVisible = false;
                StartCountdownTask();
                await InquireAsync();
            }
        }

        private void StartCountdownTask()
        {
            Task.Run(async () =>
            {
                var stopTimer = false;
                while (!stopTimer)
                {
                    if (_ct.IsCancellationRequested)
                        stopTimer = true;

                    if (--CountDown == 0)
                    {
                        _tokenSource.Cancel();
                        stopTimer = true;
                    }

                    if (!stopTimer)
                        await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }

        async Task<bool> CreateQRCodeAsync()
        {
            try
            {
                IsBusy = true;
                CountDown = GlobalSettings.Instance.PromptPayTimeout;
                IsCountdownStarted = true;
                var payload = new
                {
                    qrType = "PP",
                    invoice = $"INV{_saleEngineService.OrderData.ModifiedDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}",
                    amount = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrAmt,
                    ref1 = _refNo,
                    ref2 = _saleEngineService.LoginData.UserInfo.MachineEnv.MachineNo,
                    ref3 = _saleEngineService.LoginData.UserInfo.MachineEnv.MachineName,
                };
                QRData = await _paymentService.GeneratePPQrCode(payload);
                IsQrVisible = true;
                return true;
            }
            catch (Exception ex)
            {
                IsQrVisible = false;
                Msg = ex.Message;
                IsCloseBtnVisible = true;
            }
            finally
            {
                IsBusy = false;
            }
            return false;
        }

        async Task InquireAsync()
        {
            while (true)
            {
                if (_ct.IsCancellationRequested)
                {
                    await SetResult(null);
                    break;
                }

                try
                {
                    var result = await _paymentService.InquiryAsync(_refNo);
                    if (result != null)
                    {
                        await SetResult(result);
                        break;
                    }
                }
                catch { }
            }
        }

        async Task SetResult(PromptPayResult result)
        {
            var parameter = new NavigationParameters()
                {
                    { "PromptPayResult", result}
                };
            await GoBackAsync(parameter);
        }

        private string GetRefNo()
        {
            var orderData = _saleEngineService.OrderData;
            var shoppingCard = orderData.HeaderAttributes.Where(attr => attr.Code == "shopping_card").FirstOrDefault().ValueOfString;
            var orderNo = (int)orderData.HeaderAttributes.Where(attr => attr.Code == "order_no").FirstOrDefault().ValueOfDecimal;
            return $"{shoppingCard}{orderNo}";
        }
    }
}
