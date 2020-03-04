using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class BarcodeScanViewModel : ViewModelBase, IDialogAware
    {
        bool _isScanning;

        public BarcodeScanViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public event Action<IDialogParameters> RequestClose;

        public ICommand ScanResultCommand => new Command<ZXing.Result>((result) => ScanResult(result));

        public ICommand CancelCommand => new Command(() =>
        {
            CancelScan();
        });

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        void ScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var scanResult = result.Text;
                SetResult(scanResult);
            });
        }

        public void CancelScan()
        {
            SetResult(null);
        }

        public void SetResult(string data)
        {
            IsScanning = false;

            var parameters = new DialogParameters()
            {
                {"ScanData", data }
            };

            RequestClose(parameters);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            IsScanning = true;
        }
    }
}
