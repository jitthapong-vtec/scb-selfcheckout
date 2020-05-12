using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public class CheckerOrderViewModelBase : OrderViewModelBase
    {
        string _sessionKey;
        CustomerData _customerData;

        public CheckerOrderViewModelBase(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigationService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public string SessionKey
        {
            get => _sessionKey;
            set => SetProperty(ref _sessionKey, value);
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public ICommand SaveSessionCommand => new Command(async () =>
        {
            if (SessionData == null)
                return;

            var result = await NavigationService.ConfirmAsync(AppResources.SaveSession, AppResources.SaveSessionConfirm, AppResources.Yes, AppResources.No);
            if (!result)
                return;

            try
            {
                IsBusy = true;
                await SaveSessionAsync(SessionData.SessionKey);
                await LoadDataAsync();
                SaveSuccessCallback();
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        });

        protected virtual void SaveSuccessCallback()
        {
        }

        protected async Task<bool> LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                var isAlreadyEnd = await LoadSessionDetailAsync(SessionKey);
                if (isAlreadyEnd)
                {
                    Clear();
                    await NavigationService.ShowAlertAsync(AppResources.Opps, AppResources.SessionAlreadyFinish, AppResources.Close);
                    return false;
                }

                await LoadCustomerSession();

                CustomerData = await GetCustomerSessionAsync(SessionData.ShoppingCard);

                await LoadOrderListAsync(groupingOrderDetail: true);
            }
            catch (Exception ex)
            {
                await NavigationService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
            return true;
        }
    }
}
