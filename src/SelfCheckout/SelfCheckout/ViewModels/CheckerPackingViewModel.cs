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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CheckerPackingViewModel : OrderViewModelBase
    {
        string _sessionKey;
        CustomerData _customerData;

        public CheckerPackingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public ICommand GetSessionDetailCommand => new Command<string>(async (sessionKey) =>
            {
                if (string.IsNullOrEmpty(sessionKey))
                    return;
                SessionKey = sessionKey;
                await LoadDataAsync();
            });

        private async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                var isAlreadyEnd = await LoadSessionDetailAsync(Convert.ToInt64(SessionKey));
                if (isAlreadyEnd)
                {
                    Clear();
                    await DialogService.ShowAlert(AppResources.Alert, AppResources.SessionAlreadyFinish, AppResources.Close);
                    return;
                }

                await LoadCustomerSession();

                CustomerData = await GetCustomerSessionAsync(SessionData.ShoppingCard);

                await LoadOrderListAsync();

            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ICommand SaveSessionCommand => new Command(async () =>
        {
            if (SessionData == null)
                return;

            var result = await DialogService.ConfirmAsync(AppResources.SaveSession, AppResources.SaveSessionConfirm, AppResources.Yes, AppResources.No);
            if (!result)
                return;

            try
            {
                IsBusy = true;
                await SaveSessionAsync(SessionKey);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        });

        public ICommand ClearScreenCommand => new DelegateCommand(() =>
         {
             OrderInvoices?.Clear();
             OrderDetails?.Clear();
             SessionData = new SessionData();
             CustomerData = new CustomerData();
         });

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
