using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class ShoppingCardInputDialogViewModel : BindableBase, IDialogAware
    {
        object _lock = new object();

        public Action ShowCameraScanner;

        public event Action<IDialogParameters> RequestClose;
        string _inputValue;
        bool _isOpenScanner;

        public ShoppingCardInputDialogViewModel()
        {
            MessagingCenter.Subscribe<MainViewModel, string>(this, "ReceiveShoppingCardFromScanner", (sender, barcode) =>
            {
                InputValue = barcode;
            });
        }

        public ICommand ScanShoppingCardCommand => new DelegateCommand(() =>
         {
             lock (_lock)
             {
                 if (_isOpenScanner)
                     return;
                 else _isOpenScanner = true;
             }
             ShowCameraScanner();
         });

        public ICommand ValidateShoppingCardCommand => new DelegateCommand(() =>
        {
            var parameters = new DialogParameters()
            {
                { "ShoppingCard", InputValue }
            };
            RequestClose?.Invoke(parameters);
        });

        public ICommand CancelCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(null);
        });

        public string InputValue
        {
            get => _inputValue;
            set => SetProperty(ref _inputValue, value);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            MessagingCenter.Unsubscribe<MainViewModel, string>(this, "ReceiveShoppingCardFromScanner");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ShowCameraScanner = parameters.GetValue<Action>("ShowCameraScannerAction");
        }
    }
}
