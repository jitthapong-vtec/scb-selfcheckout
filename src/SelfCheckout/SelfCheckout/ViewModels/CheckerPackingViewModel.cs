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
        bool _canFinishSession;

        public CheckerPackingViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService) :
            base(navigationService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public bool CanFinishSession
        {
            get => _canFinishSession;
            set => SetProperty(ref _canFinishSession, value);
        }

        public ICommand GetSessionDetailCommand => new Command(async () =>
        {
            if (string.IsNullOrEmpty(SessionKey))
                return;
            CanFinishSession = await LoadDataAsync();
            if (!CanFinishSession)
                SessionKey = "";
        });

        public ICommand ClearScreenCommand => new DelegateCommand(() =>
        {
            ClearScreen();
        });

        private void ClearScreen()
        {
            SessionKey = "";
            OrderInvoices?.Clear();
            OrderDetails?.Clear();
            SessionData = new SessionData();
            CustomerData = new CustomerData();
        }

        protected override void SaveSuccessCallback()
        {
            ClearScreen();
        }
    }
}
