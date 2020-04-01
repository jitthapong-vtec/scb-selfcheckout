using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Payment;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
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
    public class PromptPayQrDialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogParameters> RequestClose;

        CancellationTokenSource _tokenSource;
        CancellationToken _ct;

        IPaymentService _paymentService;
        ISaleEngineService _saleEngineService;
        ISelfCheckoutService _selfCheckoutService;

        string _refNo;
        string _qrData;
        string _msg;
        int _countDown;
        bool _isCountdownStarted;
        bool _isBusy;
        bool _isQrVisible;

        public PromptPayQrDialogViewModel(IPaymentService paymentService, ISaleEngineService saleEngineService, ISelfCheckoutService selfCheckoutService)
        {
            _paymentService = paymentService;
            _saleEngineService = saleEngineService;
            _selfCheckoutService = selfCheckoutService;

            _tokenSource = new CancellationTokenSource();
            _ct = _tokenSource.Token;

            _refNo = GetRefNo();
        }

        public ICommand CancelCommand => new DelegateCommand(() =>
        {
            SetResult(null);
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

        public string QRData
        {
            get => _qrData;
            set => SetProperty(ref _qrData, value);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            var canGenQr = await CreateQRCodeAsync();
            if (canGenQr)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (_ct.IsCancellationRequested)
                        return false;

                    if (CountDown == 0)
                        return true;

                    if (--CountDown == 0)
                    {
                        _tokenSource.Cancel();
                        SetResult(null);
                        return false;
                    }
                    return true;
                });
                await InquireAsync();
            }
        }

        void SetResult(PromptPayResult result)
        {
            var parameter = new DialogParameters()
            {
                { "PromptPayResult", result}
            };

            RequestClose?.Invoke(parameter);
        }

        async Task<bool> CreateQRCodeAsync()
        {
            try
            {
                IsBusy = true;
                CountDown = 120;
                IsCountdownStarted = true;
                var payload = new
                {
                    qrType = "PP",
                    invoice = $"INV{_saleEngineService.OrderData.ModifiedDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}",
                    amount = _saleEngineService.OrderData.RemainingAmount.NetAmount.BaseCurrAmt,
                    ppId = "450439699596861",
                    ppType = "BILLERID",
                    ref1 = _refNo,
                    ref2 = _saleEngineService.LoginData.UserInfo.MachineEnv.MachineNo,
                    ref3 = _saleEngineService.LoginData.UserInfo.MachineEnv.MachineName,
                    merchantId = "456114188212509"
                };
                QRData = await _paymentService.GeneratePPQrCode(payload);
                IsQrVisible = true;
                return true;
            }
            catch (Exception ex)
            {
                IsQrVisible = false;
                Msg = ex.Message;
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
                    SetResult(null);
                    break;
                }

                try
                {
                    var result = await _paymentService.InquiryAsync(_refNo);
                    if (result != null)
                    {
                        SetResult(result);
                        break;
                    }
                }
                catch { }
            }
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
