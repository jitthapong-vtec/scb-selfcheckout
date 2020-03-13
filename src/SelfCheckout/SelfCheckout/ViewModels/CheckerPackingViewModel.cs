﻿using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class CheckerPackingViewModel : OrderViewModelBase
    {
        string _sessionKey;

        public CheckerPackingViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public ICommand GetSessionDetailCommand => new DelegateCommand<string>(
            canExecuteMethod: (sessionKey) => !string.IsNullOrEmpty(sessionKey),
            executeMethod:
            async (sessionKey) =>
            {
                var success = await GetSessionDetailAsync(sessionKey);
                if (success)
                {
                    SessionKey = sessionKey;

                    await GetOrderListAsync(SessionData.ShoppingCard); //TODO:
                }
            });

        public string SessionKey
        {
            get => _sessionKey;
            set => SetProperty(ref _sessionKey, value);
        }

        public override Task OnTabSelected(TabItem item)
        {
            return base.OnTabSelected(item);
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            return base.OnTabDeSelected(item);
        }
    }
}
