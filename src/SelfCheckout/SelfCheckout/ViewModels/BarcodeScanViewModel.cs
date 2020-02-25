using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class BarcodeScanViewModel : ViewModelBase
    {
        TaskCompletionSource<string> _taskCompleteSource;
        bool _isScanning;

        public override Task InitializeAsync<TViewModel, TResult>(object param, TaskCompletionSource<TResult> task)
        {
            _taskCompleteSource = task as TaskCompletionSource<string>;
            IsScanning = true;

            return base.InitializeAsync<TViewModel, TResult>(param, task);
        }

        public ICommand ScanResultCommand => new Command<ZXing.Result>((result) => ScanResult(result));

        public ICommand CancelCommand => new Command(async() =>
        {
            await CancelScan();
        });

        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                _isScanning = value;
                RaisePropertyChanged(() => IsScanning);
            }
        }

        void ScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var scanResult = result.Text;
                SetResult(scanResult);
                await NavigationService.PopModalAsync();
            });
        }

        public async Task CancelScan()
        {
            SetResult(null);
            await NavigationService.PopModalAsync();
        }

        public void SetResult(string data)
        {
            IsScanning = false;

            if (_taskCompleteSource != null)
            {
                try
                {
                    _taskCompleteSource.SetResult(data);
                }
                catch (Exception) { }
            }
        }
    }
}
