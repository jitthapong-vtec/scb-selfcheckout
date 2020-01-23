﻿using SelfCheckout.Services.Converter;
using SelfCheckout.Services.Dialog;
using SelfCheckout.Services.Identity;
using SelfCheckout.Services.Navigation;
using SelfCheckout.Services.RequestProvider;
using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels.Base
{
    public class ViewModelLocator
    {
        private static TinyIoCContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(AutoWireViewModelProperty, value);
        }

        static ViewModelLocator()
        {
            _container = new TinyIoCContainer();
            _container.Register<LandingViewModel>();
            _container.Register<LoginViewModel>();
            _container.Register<MainViewModel>();
            _container.Register<BorrowViewModel>();
            _container.Register<ShoppingCartViewModel>();

            _container.Register<INavigationService, NavigationService>();
            _container.Register<IDialogService, DialogService>();
            _container.Register<IRequestProvider, RequestProvider>();
            _container.Register<IApiResponseConverter, ApiResponseConverter>();
            _container.Register<IIdentityService, IdentityService>();
        }

        public static void RegisterSingleton<TInterface, T>() where TInterface : class where T : class, TInterface
        {
            _container.Register<TInterface, T>().AsSingleton();
        }

        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }
            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}
