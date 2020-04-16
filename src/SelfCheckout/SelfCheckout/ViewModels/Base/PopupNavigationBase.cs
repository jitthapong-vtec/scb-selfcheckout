using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class PopupNavigationBase<TResult> : BindableBase, INavigationAware
    {
        public INavigationService NavigationService { get; }
        public TaskCompletionSource<TResult> TaskResult;

        public PopupNavigationBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        protected async Task GoBackAsync(TResult result)
        {
            await NavigationService.GoBackAsync();
            TaskResult?.SetResult(result);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            TaskResult = parameters.GetValue<TaskCompletionSource<TResult>>("TaskResult");
        }
    }
}
