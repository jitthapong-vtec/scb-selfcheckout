﻿using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class AuthorizationDialogViewModel : AuthorizationViewModelBase, IDialogAware
    {
        public AuthorizationDialogViewModel(ISaleEngineService saleEngineService, ISelfCheckoutService selfCheckoutService) : base(saleEngineService, selfCheckoutService)
        {
        }

        protected override Task AuthorizeCallback(LoginData loginData)
        {
            SetResult(true, loginData);
            return Task.FromResult(true);
        }

        public ICommand CancelCommand => new DelegateCommand(() =>
        {
            SetResult(false, null);
        });

        public event Action<IDialogParameters> RequestClose;

        void SetResult(bool isAuthorized, LoginData loginData)
        {
            var parameters = new DialogParameters()
            {
                {"IsAuthorized", isAuthorized },
                {"LoginData", loginData }
            };
            RequestClose?.Invoke(parameters);
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
        }
    }
}
