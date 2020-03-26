﻿using SelfCheckout.Resources;
using SelfCheckout.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentProcessView : ContentView
    {
        public PaymentProcessView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MainViewModel>(this, "LanguageChanged", (sender) =>
            {
                lblCountdown.Text = AppResources.CountDown;
                lblPayment.Text = AppResources.Payment;
                lblScanPayment.Text = AppResources.PleaseScanPayment;
            });
        }
    }
}