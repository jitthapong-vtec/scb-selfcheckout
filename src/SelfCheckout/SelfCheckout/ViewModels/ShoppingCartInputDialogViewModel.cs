using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class ShoppingCardInputDialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogParameters> RequestClose;
        string _inputValue;

        IDialogService _dialogService;

        public ShoppingCardInputDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public ICommand ScanShoppingCardCommand => new DelegateCommand(() =>
         {
             _dialogService.ShowDialog("BarcodeScanDialog", null, (scanResult) =>
             {
                 var result = scanResult.Parameters.GetValue<string>("ScanData");
                 if (!string.IsNullOrEmpty(result))
                 {
                     try
                     {
                         var definition = new { S = "", C = "" };
                         var qrFromKiosk = JsonConvert.DeserializeAnonymousType(result, definition);
                         InputValue = qrFromKiosk.S;
                     }
                     catch
                     {
                         InputValue = result;
                     }
                 }
             });
         });

        public ICommand ValidateShoppingCardCommand => new DelegateCommand(() =>
        {
            var parameters = new DialogParameters()
            {
                { "ShoppingCard", InputValue }
            };
            RequestClose(parameters);
        });

        public ICommand CancelCommand => new DelegateCommand(() =>
        {
            RequestClose(null);
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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
