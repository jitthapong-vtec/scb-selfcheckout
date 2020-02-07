using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string title, string message, string buttonText)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonText);
        }

        public Task<bool> ShowConfirmAsync(string title, string message, string okText, string cancelText)
        {
            return UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText, null);
        }
    }
}
