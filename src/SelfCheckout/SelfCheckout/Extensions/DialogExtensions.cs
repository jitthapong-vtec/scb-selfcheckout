using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Extensions
{
    public static class DialogExtensions
    {
        public static void ShowAlert(this IDialogService dialogService, string title, string message, string okButtonText = "Close")
        {
            var parameters = new DialogParameters
            {
                { "Title", title },
                { "Message", message },
                {"OkButtonText", okButtonText }
            };
            dialogService.ShowDialog("CommonDialog", parameters);
        }

        public static Task<bool> ConfirmAsync(this IDialogService dialogService, string title, string message, string okButtonText, string cancelButtonText)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var parameters = new DialogParameters
                {
                    { "Title", title },
                    { "Message", message },
                    { "OkButtonText", okButtonText },
                    { "CancelButtonText", cancelButtonText }
                };
                void Callback(IDialogResult result)
                {
                    tcs.SetResult(result.Parameters.GetValue<bool>("confirmed"));
                }
                dialogService.ShowDialog("ConfirmDialog", parameters, Callback);
            });
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
