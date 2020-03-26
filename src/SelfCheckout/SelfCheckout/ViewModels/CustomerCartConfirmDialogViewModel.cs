using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CustomerCartConfirmDialogViewModel : BindableBase, IDialogAware
    {
        Person _person;

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Person = parameters.GetValue<Person>("Person");
        }

        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        public ICommand ConfirmCommand => new Command(() =>
        {
            SetResult(true);
        });

        public ICommand CancelCommand => new Command(() =>
        {
            SetResult(false);
        });

        void SetResult(bool isConfirm)
        {
            var parameters = new DialogParameters()
            {
                {"IsConfirm", isConfirm }
            };
            RequestClose(parameters);
        }
    }
}
