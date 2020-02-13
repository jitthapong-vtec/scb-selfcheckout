using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.Session;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class BorrowViewModel : ViewModelBase
    {
        ISessionService _sessionService;
        IRegisterService _registerService;

        string _inputValue = "1910001252256";
        Person _person;

        bool _isCfPopupVisible;

        public BorrowViewModel(ISessionService sessionService, IRegisterService registerService)
        {
            _sessionService = sessionService;
            _registerService = registerService;
        }

        public ICommand ScanShoppingCartCommand => new Command(async () => await ScanShoppingCartAsync());

        public ICommand ValidateShoppingCartCommand => new Command(async () => await ValidateShoppingCartAsync());

        public ICommand ConfirmCommand => new Command(async () => await ConfirmAsync());

        public ICommand CancelCommand => new Command(() => IsCfPopupVisible = false);

        async Task ScanShoppingCartAsync()
        {
            var task = new TaskCompletionSource<string>();
            await NavigationService.NavigateToAsync<BarcodeScanViewModel, string>(null, task);
            var result = await task.Task;
            if (!string.IsNullOrEmpty(result))
            {
                InputValue = result;
                await ValidateShoppingCartAsync();
            }
        }

        async Task ValidateShoppingCartAsync()
        {
            try
            {
                IsBusy = true;
                var validateResult = await _sessionService.ValidateShoppingCartAsync("172.X.2.006", InputValue);
                if (!validateResult.IsCompleted)
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, validateResult.DefaultMessage, AppResources.Close);
                    return;
                }

                var payload = new
                {
                    shoppingCard = "3600000711400",
                    SubBranch = "MHN-MA-DT",
                    pickupCode = "",
                    isTour = false,
                    platform = "FRMFIT",
                    isGenPdfPromotion = false,
                    isGenImgShoppingCard = false
                };

                var customerDataResult = await _registerService.GetCustomerAsync(payload);
                if (!customerDataResult.IsCompleted)
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, validateResult.DefaultMessage, AppResources.Close);
                    return;
                }
                Person = _registerService.CustomerData?.Person;
                IsCfPopupVisible = true;
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ConfirmAsync()
        {
            try
            {
                var startResult = await _sessionService.StartSessionAsync("test_user", "99908", InputValue);
                if (!startResult.IsCompleted)
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, startResult.DefaultMessage, AppResources.Close);
                    return;
                }
                await NavigationService.NavigateToAsync<MainViewModel>();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);
            }
        }

        public string InputValue
        {
            get => _inputValue;
            set
            {
                _inputValue = value;
                RaisePropertyChanged(() => InputValue);
            }
        }

        public Person Person
        {
            get => _person;
            set
            {
                _person = value;
                RaisePropertyChanged(() => Person);
            }
        }

        public bool IsCfPopupVisible
        {
            get => _isCfPopupVisible;
            set
            {
                _isCfPopupVisible = value;
                RaisePropertyChanged(() => IsCfPopupVisible);
            }
        }
    }
}
