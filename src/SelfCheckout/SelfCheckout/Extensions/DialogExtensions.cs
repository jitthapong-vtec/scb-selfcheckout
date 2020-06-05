using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Extensions
{
    public static class DialogExtensions
    {
        static object _lock = new object();
        static bool _isConfirmShowing;

        public static async Task<INavigationParameters> ShowAlertAsync(this INavigationService navigationService, string title, string message, string okButtonText = "Close")
        {
            var tcs = new TaskCompletionSource<INavigationParameters>();
            var parameters = new NavigationParameters
            {
                { "Title", title },
                { "Message", message },
                { "TaskResult", tcs },
                { "OkButtonText", okButtonText }
            };
            try
            {
                await navigationService.NavigateAsync("AlertDialog", parameters);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return await tcs.Task;
        }

        public static async Task<bool> ConfirmAsync(this INavigationService navigationService, string title, string message, string okButtonText, string cancelButtonText, bool okAsRedButton = false)
        {
            lock (_lock)
            {
                if (_isConfirmShowing)
                    return false;
                else
                    _isConfirmShowing = true;
            }

            var tcs = new TaskCompletionSource<bool>();
            var parameters = new NavigationParameters
                {
                    { "Title", title },
                    { "Message", message },
                    { "TaskResult", tcs },
                    { "OkButtonText", okButtonText },
                    { "CancelButtonText", cancelButtonText },
                    { "OkAsRedButton", okAsRedButton }
                };
            try
            {
                await navigationService.NavigateAsync("ConfirmDialog", parameters);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            var result = await tcs.Task;
            _isConfirmShowing = false;
            return result;
        }

        public static async Task<TResult> ShowDialogAsync<TResult>(this INavigationService navigationService, string name, INavigationParameters parameters)
        {
            var tcs = new TaskCompletionSource<TResult>();
            try
            {
                parameters = parameters ?? new NavigationParameters();
                parameters.Add("TaskResult", tcs);
                await navigationService.NavigateAsync(name, parameters);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return await tcs.Task;
        }
    }
}
