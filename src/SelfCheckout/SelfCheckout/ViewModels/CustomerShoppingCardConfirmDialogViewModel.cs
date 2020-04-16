using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CustomerShoppingCardConfirmDialogViewModel : PopupNavigationBase<INavigationParameters>
    {
        Person _person;

        public CustomerShoppingCardConfirmDialogViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        public ICommand ConfirmCommand => new Command(async () =>
        {
            await SetResult(true);
        });

        public ICommand CancelCommand => new Command(async () =>
        {
            await SetResult(false);
        });

        async Task SetResult(bool result)
        {
            var parameters = new NavigationParameters()
            {
                {"IsConfirm", result }
            };
            await GoBackAsync(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Person = parameters.GetValue<Person>("Person");
        }
    }
}
