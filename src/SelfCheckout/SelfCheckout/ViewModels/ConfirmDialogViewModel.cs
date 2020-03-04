using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class ConfirmDialogViewModel : BindableBase, IDialogAware
    {
        string _title;
        string _message;
        string _yesButtonText;
        string _noButtonText;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string YesButtonText
        {
            get => _yesButtonText;
            set => SetProperty(ref _yesButtonText, value);
        }

        public string NoButtonText
        {
            get => _noButtonText;
            set => SetProperty(ref _noButtonText, value);
        }

        public ICommand YesCommand => new DelegateCommand(() =>
        {
            SetResult(true);
        });

        public ICommand NoCommand => new DelegateCommand(() =>
        {
            SetResult(false);
        });

        public event Action<IDialogParameters> RequestClose;

        void SetResult(bool isConfirm)
        {
            var parameters = new DialogParameters()
            {
                {"IsConfirm", isConfirm }
            };

            RequestClose(parameters);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = parameters.GetValue<string>("Title");
            Message = parameters.GetValue<string>("Message");
            YesButtonText = parameters.GetValue<string>("OkButtonText");
        }
    }
}
