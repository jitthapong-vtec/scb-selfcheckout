using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class SessionOrderDialogViewModel : OrderViewModelBase, IDialogAware
    {
        public event Action<IDialogParameters> RequestClose;

        CustomerData _customerData;

        public SessionOrderDialogViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose(null);
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            var sessionKey = parameters.GetValue<string>("SessionKey");
            var shoppingCard = parameters.GetValue<string>("ShoppingCard");

            try
            {
                IsBusy = true;
                await LoadSessionDetailAsync(sessionKey);
                CustomerData = await GetCustomerSessionAsync(shoppingCard);

                await LoadOrderListAsync();

            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
