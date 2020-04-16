using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class SessionOrderDialogViewModel : CheckerOrderViewModelBase
    {
        TaskCompletionSource<INavigationParameters> _tcs;

        public SessionOrderDialogViewModel(INavigationService navigationService, ISelfCheckoutService selfCheckoutService,
            ISaleEngineService saleEngineService, IRegisterService registerService) :
            base(navigationService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public ICommand ConfirmCommand => new DelegateCommand(async () =>
        {
            var parameters = new NavigationParameters()
            {
                {"IsConfirmed", true },
                {"OrderInvoices", OrderInvoices }
            };
            await SetResult(parameters);
        });

        public ICommand CancelCommand => new DelegateCommand(async () =>
        {
            await SetResult(null);
        });

        async Task SetResult(INavigationParameters parameters)
        {
            _tcs?.SetResult(parameters);
            await NavigationService.GoBackAsync();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _tcs = parameters.GetValue<TaskCompletionSource<INavigationParameters>>("TaskResult");

            var sessionKey = parameters.GetValue<long>("SessionKey");
            var shoppingCard = parameters.GetValue<string>("ShoppingCard");
            var sessionDate = parameters.GetValue<DateTime>("SessionDate");

            try
            {
                IsBusy = true;
                await LoadSessionDetailAsync(sessionKey);
                await LoadCustomerSession();

                CustomerData = await GetCustomerSessionAsync(shoppingCard);

                object filter = null;
                /*
                if (sessionDate != null)
                {
                    filter = new object[]
                    {
                        new
                        {
                            sign = "string",
                            element = "order_data",
                            option = "string",
                            type = "string",
                            low = sessionDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            height = sessionDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        }
                    };
                }*/
                await LoadOrderListAsync(filter);
                GroupingOrder();
            }
            catch { }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
