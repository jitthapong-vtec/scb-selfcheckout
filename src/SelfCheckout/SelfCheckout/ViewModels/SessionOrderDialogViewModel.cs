﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class SessionOrderDialogViewModel : OrderViewModelBase, IDialogAware
    {
        public event Action<IDialogParameters> RequestClose;

        CustomerData _customerData;

        public SessionOrderDialogViewModel(IDialogService dialogService, ISelfCheckoutService selfCheckoutService, 
            ISaleEngineService saleEngineService, IRegisterService registerService) : 
            base(dialogService, selfCheckoutService, saleEngineService, registerService)
        {
        }

        public CustomerData CustomerData
        {
            get => _customerData;
            set => SetProperty(ref _customerData, value);
        }

        public ICommand ConfirmCommand => new DelegateCommand(() =>
        {
            var dialogParameter = new DialogParameters()
            {
                {"IsConfirmed", true },
                {"OrderInvoices", OrderInvoices }
            };
            RequestClose?.Invoke(dialogParameter);
        });

        public ICommand CancelCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(null);
        });

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            var sessionKey = parameters.GetValue<long>("SessionKey");
            var shoppingCard = parameters.GetValue<string>("ShoppingCard");
            var sessionDate = parameters.GetValue<DateTime>("SessionDate");

            try
            {
                IsBusy = true;
                await LoadSessionDetailAsync(sessionKey);
                await LoadCustomerSession();

                CustomerData = await GetCustomerSessionAsync(shoppingCard);

                object filter = null;
                /*
                if (sessionDate != null)
                {
                    filter = new object[]
                    {
                        new
                        {
                            sign = "string",
                            element = "order_data",
                            option = "string",
                            type = "string",
                            low = sessionDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            height = sessionDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        }
                    };
                }*/
                await LoadOrderListAsync(filter);
            }
            catch { }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
