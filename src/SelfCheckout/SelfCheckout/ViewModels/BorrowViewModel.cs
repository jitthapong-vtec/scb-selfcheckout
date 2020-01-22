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

        Task ScanPassportAsync()
        {
            return Task.FromResult(false);
        }

        Task CheckPassportAsync()
        {
            IsCfPopupVisible = !IsCfPopupVisible;
            return Task.FromResult(false);
        }

        Task ConfirmAsync()
        {
            return Task.FromResult(false);
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
