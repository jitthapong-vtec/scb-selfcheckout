using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CameraScannerViewModel : NavigatableViewModelBase
    {
        bool _isScanning;

        public CameraScannerViewModel(INavigationService navigationService, IDialogService dialogService) : base(navigationService, dialogService)
        {
        }

        public ICommand ScanResultCommand => new Command<ZXing.Result>((result) => ScanResult(result));

        public ICommand CancelCommand => new Command(() =>
        {
            CancelScan();
        });

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            IsScanning = true;
        }

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        void ScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var scanResult = result.Text;
                await SetResultAsync(scanResult);
            });
        }

        public async void CancelScan()
        {
            await SetResultAsync(null);
        }

        public async Task SetResultAsync(string data)
        {
            IsScanning = false;

            var parameters = new NavigationParameters()
            {
                {"ScanData", data }
            };
            await NavigationService.GoBackAsync(parameters);
        }
    }
}
