using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string buttonText = "Close");

        Task<bool> ShowConfirmAsync(string title, string message, string okText = "Ok", string cancelText = "Cancel");
    }
}
