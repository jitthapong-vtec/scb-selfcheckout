using Prism.Mvvm;
using Prism.Services.Dialogs;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CouponInputViewModel : ViewModelBase
    {
        public Func<Task> OpenCameraScannerAsync;
        public Func<string, Task> SetCouponAsync;

        string _couponCode;
        string _couponCodeDsp;

        bool _couponInputViewVisible;

        public CouponInputViewModel(IDialogService dialogService) : base(dialogService)
        {
        }

        public ICommand OpenCameraScannerCommand => new Command(() => OpenCameraScannerAsync?.Invoke());

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
            set => SetProperty(ref _couponInputViewVisible, value);
        }
    }
}
