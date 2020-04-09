using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Print;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CheckerPackingViewModel : CheckerOrderViewModelBase
    {
        public CheckerPackingViewModel(IDialogService dialogService, ISelfCheckoutService selfCheckoutService, 
            ISaleEngineService saleEngineService, IRegisterService registerService) : 
            base(dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public ICommand GetSessionDetailCommand => new Command<string>(async (sessionKey) =>
            {
                if (string.IsNullOrEmpty(sessionKey))
                    return;
                SessionKey = sessionKey;
                await LoadDataAsync();
            });

        public ICommand ClearScreenCommand => new DelegateCommand(() =>
         {
             OrderInvoices?.Clear();
             OrderDetails?.Clear();
             SessionData = new SessionData();
             CustomerData = new CustomerData();
         });
    }
}
