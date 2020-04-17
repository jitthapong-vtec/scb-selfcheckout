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
            try
            {
                var navigationParameters = new NavigationParameters()
                {
                    {"BackFromConfirm", true }
                };
                await NavigationService.GoBackAsync(navigationParameters);
                TaskResult?.SetResult(result);
            }
            catch (Exception ex)
            {

            }
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
