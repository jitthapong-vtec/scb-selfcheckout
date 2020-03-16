using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class CheckerPackingViewModel : OrderViewModelBase
    {
        string _sessionKey;
        SessionData _sessionData;
        CustomerData _customerData;

        public CheckerPackingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public ICommand GetSessionDetailCommand => new DelegateCommand<string>(
            canExecuteMethod: (sessionKey) => !string.IsNullOrEmpty(sessionKey),
            executeMethod:
            async (sessionKey) =>
            {
                try
                {
                    SessionData = await GetSessionDetailAsync(sessionKey);
                    CustomerData = await GetCustomerSessionAsync(SessionData.ShoppingCard);
                    SessionKey = sessionKey;

                    await LoadOrderListAsync(SessionData.ShoppingCard);

                }
                catch(Exception ex) 
                {
                    DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
                }
            });

        public string SessionKey
        {
            get => _sessionKey;
            set => SetProperty(ref _sessionKey, value);
        }

        public SessionData SessionData
        {
            get => _sessionData;
            set => SetProperty(ref _sessionData, value);
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public override Task OnTabSelected(TabItem item)
        {
            return base.OnTabSelected(item);
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            return base.OnTabDeSelected(item);
        }
    }
}
