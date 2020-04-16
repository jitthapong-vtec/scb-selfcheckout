using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class ShoppingCardInputDialogViewModel : PopupNavigationBase<INavigationParameters>
    {
        object _lock = new object();

        string _inputValue;
        bool _isOpenScanner;

        public ShoppingCardInputDialogViewModel(INavigationService navigationService) : base(navigationService)
        {
            MessagingCenter.Subscribe<MainViewModel, string>(this, "ReceiveShoppingCardFromScanner", (sender, barcode) =>
            {
                if (!string.IsNullOrEmpty(barcode))
                {
                    try
                    {
                        var definition = new { S = "", C = "" };
                        var qrFromKiosk = JsonConvert.DeserializeAnonymousType(barcode, definition);
                        InputValue = qrFromKiosk.S;
                    }
                    catch
                    {
                        InputValue = barcode;
                    }
                }
            });
        }

        public ICommand ScanShoppingCardCommand => new DelegateCommand(async () =>
         {
             lock (_lock)
             {
                 if (_isOpenScanner)
                     return;
                 else _isOpenScanner = true;
             }
             var result = await NavigationService.ShowDialogAsync<string>("CameraScannerView", null);
             InputValue = result;
         });

        public ICommand ValidateShoppingCardCommand => new DelegateCommand(async () =>
        {
            await SetResult(InputValue);
        });

        public ICommand CancelCommand => new DelegateCommand(async () =>
        {
            await SetResult(null);
        });

        async Task SetResult(string result)
        {
            var parameters = new NavigationParameters()
            {
                { "ShoppingCard", result }
            };
            await GoBackAsync(parameters);
        }

        public string InputValue
        {
            get => _inputValue;
            set => SetProperty(ref _inputValue, value);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            MessagingCenter.Unsubscribe<MainViewModel, string>(this, "ReceiveShoppingCardFromScanner");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
