using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class BorrowViewModel : ViewModelBase
    {
        bool _isCfPopupVisible;

        public ICommand ScanPassportCommand => new Command(async () => await ScanPassportAsync());

        public ICommand CheckPassportCommand => new Command(async () => await CheckPassportAsync());

        public ICommand ConfirmCommand => new Command(async () => await ConfirmAsync());

        public ICommand CancelCommand => new Command(() => IsCfPopupVisible = false);

        async Task ScanPassportAsync()
        {
            var task = new TaskCompletionSource<string>();
            await NavigationService.NavigateToAsync<BarcodeScanViewModel, string>(null, task);
            var result = task.Task;
            await CheckPassportAsync();
        }

        Task CheckPassportAsync()
        {
            IsCfPopupVisible = !IsCfPopupVisible;
            return Task.FromResult(false);
        }

        async Task ConfirmAsync()
        {
            await NavigationService.NavigateToAsync<MainViewModel>();
        }

        public bool IsCfPopupVisible {
            get => _isCfPopupVisible;
            set
            {
                _isCfPopupVisible = value;
                RaisePropertyChanged(() => IsCfPopupVisible);
            }
        }
    }
}
