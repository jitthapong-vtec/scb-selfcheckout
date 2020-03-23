using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Print;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CheckerSettingViewModel : ViewModelBase
    {
        Printer _printer;

        public CheckerSettingViewModel(INavigationService navigatinService, IDialogService dialogService) : base(navigatinService, dialogService)
        {
        }

        public ICommand FindPrinterCommand => new DelegateCommand(async () =>
        {
            Printer = await DependencyService.Get<IPrintService>().FindAndClaimPrinter();

            Preferences.Set("PrinterId", Printer.PrinterDeviceId);
        });

        public Printer Printer
        {
            get => _printer;
            set => SetProperty(ref _printer, value);
        }
    }
}
