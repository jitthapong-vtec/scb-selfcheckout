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
            Task.Factory.StartNew(() =>
            {
                var parameters = new DialogParameters
                {
                    { "Title", title },
                    { "Message", message },
                    {"OkButtonText", okButtonText },
                    {"CancelButtonText", cancelButtonText }
                };
                void Callback(IDialogResult result)
                {
                    tcs.SetResult(result.Parameters.GetValue<bool>("confirmed"));
                }
                dialogService.ShowDialog("ConfirmDialog", parameters, Callback);
            });
            return tcs.Task;
        }
    }
}
