using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Print;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CheckerSettingViewModel : SettingViewModelBase
    {
        string _printerName;

        public CheckerSettingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService, selfCheckoutService)
        {
        }

        public string PrinterName
        {
            get => _printerName;
            set => SetProperty(ref _printerName, value);
        }

        public override async void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            await DependencyService.Get<IPrintService>().SavePrinterName(PrinterName);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            PrinterName = await DependencyService.Get<IPrintService>().GetSavedPrinterName();
        }
    }
}
