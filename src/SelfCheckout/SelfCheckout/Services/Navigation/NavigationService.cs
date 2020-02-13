using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using SelfCheckout.ViewModels;
using SelfCheckout.ViewModels.Base;
using SelfCheckout.Views;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SelfCheckout.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        public Task InitializeAsync()
        {
            return NavigateToAsync<LandingViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task RemoveLastFromBackStackAsync()
        {
            var mainPage = Application.Current.MainPage as CustomNavigationView;

            if (mainPage != null)
            {
                mainPage.Navigation.RemovePage(
                    mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        public async Task PopBackAsync()
        {
            var mainPage = Application.Current.MainPage as CustomNavigationView;
            if (mainPage != null)
            {
                await mainPage.Navigation.PopAsync();
            }
        }

        public Task RemoveBackStackAsync()
        {
            var mainPage = Application.Current.MainPage as CustomNavigationView;

            if (mainPage != null)
            {
                for (int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++)
                {
                    var page = mainPage.Navigation.NavigationStack[i];
                    mainPage.Navigation.RemovePage(page);
                }
            }
            return Task.FromResult(true);
        }

        private async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreatePage(viewModelType, parameter);
            if (page is LandingView || page is LoginView || page is MainView)
            {
                Application.Current.MainPage = new CustomNavigationView(page);
            }
            else
            {
                var navigationPage = Application.Current.MainPage as CustomNavigationView;
                if (navigationPage == null)
                    Application.Current.MainPage = new CustomNavigationView(page);
                else
                    await navigationPage.PushAsync(page);
            }
            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }
            return Activator.CreateInstance(pageType) as Page;
        }

        public async Task NavigateToAsync<TViewModel, TResult>(object parameter, TaskCompletionSource<TResult> task) where TViewModel : ViewModelBase
        {
            var page = CreatePage(typeof(TViewModel), parameter);
            var navigationPage = Application.Current.MainPage as CustomNavigationView;
            if (navigationPage != null)
            {
                await navigationPage.PushAsync(page);
            }
            else
            {
                Application.Current.MainPage = new CustomNavigationView(page);
            }
            var viewModel = page.BindingContext as ViewModelBase;
            await viewModel.InitializeAsync<TViewModel, TResult>(parameter, task);
        }

        public Task SendPopbackMessage()
        {
            MessagingCenter.Send(this, MessageKeys.NavigationPopback);
            return Task.FromResult(true);
        }

        public async Task PushModalAsync<TViewModel, TResult>(object parameter, TaskCompletionSource<TResult> task) where TViewModel : ViewModelBase
        {
            var page = (PopupPage)CreatePage(typeof(TViewModel), parameter);
            await Application.Current.MainPage.Navigation.PushPopupAsync(page, true);
            var viewModel = page.BindingContext as ViewModelBase;
            await viewModel.InitializeAsync<TViewModel, TResult>(parameter, task);
        }

        public async Task PopModalAsync()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopAllPopupAsync(true);
            }
            catch (Exception) { }
        }
    }
}
