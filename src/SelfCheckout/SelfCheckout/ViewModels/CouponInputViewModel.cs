using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CouponInputViewModel : NavigatableViewModelBase
    {
        public Func<string, Task> SetCouponAsync;

        string _couponCode;
        string _couponCodeDsp;

        bool _isOpenCameraScanner;
        bool _couponInputViewVisible;

        object _lockScanner = new object();

        public CouponInputViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public ICommand OpenCameraScannerCommand => new Command(async () =>
        {
            lock (_lockScanner)
            {
                if (_isOpenCameraScanner)
                    return;
                else
                    _isOpenCameraScanner = true;
            }
            var result = await NavigationService.ShowDialogAsync<string>("CameraScannerView", null);
            CouponCode = result;
            _isOpenCameraScanner = false;
        });

        public ICommand OpenHardwareScannerCommand => new Command(() => MessagingCenter.Send<ViewModelBase>(this, "RequestHWScanner"));

        public ICommand HideCouponInputCommand => new Command(() => CouponInputViewVisible = false);

        public string CouponCode
        {
            get => _couponCode;
            set => SetProperty(ref _couponCode, value, async () =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    CouponCodeDsp = "Scanned";
                    await SetCouponAsync?.Invoke(value);
                }
                else
                {
                    CouponCodeDsp = value;
                }
            });
        }

        public string CouponCodeDsp
        {
            get => _couponCodeDsp;
            set => SetProperty(ref _couponCodeDsp, value);
        }

        public bool CouponInputViewVisible
        {
            get => _couponInputViewVisible;
            set => SetProperty(ref _couponInputViewVisible, value, () =>
            {
                CouponCode = "";
                CouponCodeDsp = "";
            });
        }
    }
}
