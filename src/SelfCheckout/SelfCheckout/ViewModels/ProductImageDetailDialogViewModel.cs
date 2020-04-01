using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class ProductImageDetailDialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogParameters> RequestClose;

        string _imageUrl;

        public ICommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(null);
        });

        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
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
            ImageUrl = parameters.GetValue<string>("ImageUrl");
        }
    }
}
