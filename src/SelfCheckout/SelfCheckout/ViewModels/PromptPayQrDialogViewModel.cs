﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Payment;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                var refNo = GetRefNo();
                var payload = new
                {
                    qrType = "PP",
                    invoice = $"{_saleEngineService.LoginData.UserInfo.MachineEnv.MachineNo}{refNo}",
                    amount = _saleEngineService.OrderData.TotalBillingAmount.NetAmount.CurrAmt,
                    ppId = "450439699596861",
                    ppType = "BILLERID",
                    ref1 = refNo,
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
            if (_ct.IsCancellationRequested)
            {
                SetResult(null);
                return;
            }

            try
            {
                var refNo = GetRefNo();
                var result = await _paymentService.InquiryAsync(refNo);
                if (result != null)
                    SetResult(result);
                else
                    await InquireAsync();
            }
            finally
            {
                await InquireAsync();
            }
        }

        private string GetRefNo()
        {
            return _saleEngineService.OrderData.CreateDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }
    }
}
