using Newtonsoft.Json;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
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
        string _inputValue = "9898989898989";

        public ICommand ScanShoppingCartCommand => new Command(async () => await ScanShoppingCartAsync());

        public ICommand ValidateShoppingCartCommand => new Command(async () => await ValidateShoppingCartAsync());

        async Task ScanShoppingCartAsync()
        {
            var task = new TaskCompletionSource<string>();
            await NavigationService.PushModalAsync<BarcodeScanViewModel, string>(null, task);
            var result = await task.Task;
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    var definition = new { S = "", C = "" };
                    var qrFromKiosk = JsonConvert.DeserializeAnonymousType(result, definition);
                    InputValue = qrFromKiosk.S;
                }
                catch
                {
                    InputValue = result;
                }
            }
        }

        async Task ValidateShoppingCartAsync()
        {
            try
            {
                IsBusy = true;
                var validateResult = await SelfCheckoutService.ValidateShoppingCartAsync(LoginData.UserInfo.MachineEnv.MachineIp, InputValue);
                if (!validateResult.IsCompleted)
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, validateResult.DefaultMessage, AppResources.Close);
                    return;
                }

                SelfCheckoutService.CurrentShoppingCart = InputValue;

                var payload = new
                {
                    shoppingCard = InputValue,
                    SubBranch = AppConfig.SubBranch,
                    isTour = false,
                    isGenPdfPromotion = false,
                    isGenImgShoppingCard = false
                };

                var customerDataResult = await RegisterService.GetCustomerAsync(payload);
                if (!customerDataResult.IsCompleted)
                {
                    await DialogService.ShowAlertAsync(AppResources.Opps, validateResult.DefaultMessage, AppResources.Close);
                    return;
                }

                if (CustomerData?.Person != null)
                {
                    //if (!CustomerData.Person.IsActivate)
                    //{
                    //    await DialogService.ShowAlertAsync(AppResources.Opps, $"{CustomerData.Person.NativeName} is not activate!", AppResources.Close);
                    //    return;
                    //}
                    var task = new TaskCompletionSource<bool>();
                    await NavigationService.PushModalAsync<CustomerCartConfirmViewModel, bool>(CustomerData.Person, task);
                    var result = await task.Task;
                    if (result)
                        await StartSessionAsync();
                }
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

        async Task StartSessionAsync()
        {
            try
            {
                IsBusy = true;
                var startResult = await SelfCheckoutService.StartSessionAsync(LoginData.UserInfo.UserCode, LoginData.UserInfo.MachineEnv.MachineNo, InputValue);
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
            finally
            {
                IsBusy = false;
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
    }
}
