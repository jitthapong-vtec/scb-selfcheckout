using SelfCheckout.ViewModels.Base;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Navigation
{
    public interface INavigationService
    {
        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task NavigateToAsync<TViewModel, TResult>(object parameter, TaskCompletionSource<TResult> task) where TViewModel : ViewModelBase;

        Task PushModalAsync<TViewModel, TResult>(object parameter, TaskCompletionSource<TResult> task) where TViewModel : ViewModelBase;

        Task RemoveLastFromBackStackAsync();

        Task RemoveBackStackAsync();

        Task PopBackAsync();

        Task PopModalAsync();

        Task SendPopbackMessage();
    }
}
