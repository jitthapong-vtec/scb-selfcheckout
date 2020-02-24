using SelfCheckout.Models;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class CustomerCartConfirmViewModel : ViewModelBase
    {
        TaskCompletionSource<bool> _task;

        Person _person;

        public override Task InitializeAsync<TViewModel, TResult>(object param, TaskCompletionSource<TResult> task)
        {
            _task = task as TaskCompletionSource<bool>;
            Person = param as Person;
            return base.InitializeAsync<TViewModel, TResult>(param, task);
        }

        public Person Person
        {
            get => _person;
            set
            {
                _person = value;
                RaisePropertyChanged(() => Person);
            }
        }

        public ICommand ConfirmCommand => new Command(async () =>
        {
            await NavigationService.PopModalAsync();
            _task.SetResult(true);
        });

        public ICommand CancelCommand => new Command(async () =>
        {
            await NavigationService.PopModalAsync();
            _task.SetResult(false);
        });

    }
}
