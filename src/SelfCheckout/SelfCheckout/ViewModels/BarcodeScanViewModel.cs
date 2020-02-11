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

        public override Task InitializeAsync<TViewModel, TResult>(object param, TaskCompletionSource<TResult> task)
        {
            _taskCompleteSource = task as TaskCompletionSource<string>;

            return base.InitializeAsync<TViewModel, TResult>(param, task);
        }

        public ICommand ScanResultCommand => new Command<ZXing.Result>((result) => ScanResult(result));

        void ScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var scanResult = result.Text;
                SetResult(scanResult);
                await NavigationService.PopBackAsync();
            });
        }

        public void SetResult(string data)
        {
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
