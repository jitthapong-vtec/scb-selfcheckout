using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Extensions
{
    public static class DialogExtensions
    {
        public static Task<IDialogResult> ShowAlert(this IDialogService dialogService, string title, string message, string okButtonText = "Close")
        {
            var parameters = new DialogParameters
            {
                { "Title", title },
                { "Message", message },
                { "OkButtonText", okButtonText }
            };
            var tcs = new TaskCompletionSource<IDialogResult>();
            try
            {
                dialogService.ShowDialog("AlertDialog", parameters, (result) =>
                {
                    if (result.Exception != null)
                    {
                        tcs.SetException(result.Exception);
                        return;
                    }
                    tcs.SetResult(result);
                });
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public static Task<bool> ConfirmAsync(this IDialogService dialogService, string title, string message, string okButtonText, string cancelButtonText, bool okAsRedButton = false)
        {
            var tcs = new TaskCompletionSource<bool>();
            var parameters = new DialogParameters
                {
                    { "Title", title },
                    { "Message", message },
                    { "OkButtonText", okButtonText },
                    { "CancelButtonText", cancelButtonText },
                    { "OkAsRedButton", okAsRedButton }
                };
            try
            {
                dialogService.ShowDialog("ConfirmDialog", parameters, (result) =>
                {
                    if (result.Exception != null)
                    {
                        tcs.SetException(result.Exception);
                    }
                    tcs.SetResult(result.Parameters.GetValue<bool>("IsConfirmed"));
                });
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public static Task<IDialogResult> ShowDialogAsync(this IDialogService dialogService, string name, IDialogParameters parameters)
        {
            var tcs = new TaskCompletionSource<IDialogResult>();
            try
            {
                dialogService.ShowDialog(name, parameters, (result) =>
                {
                    if (result.Exception != null)
                    {
                        tcs.SetException(result.Exception);
                        return;
                    }
                    tcs.SetResult(result);
                });
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }
    }
}
