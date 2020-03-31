using SelfCheckout.Resources;
using SelfCheckout.ViewModels;
using SelfCheckout.ViewModels.Base;
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

            MessagingCenter.Subscribe<MainViewModel>(this, ViewModelBase.MessageKey_LanguageChanged, (sender) =>
            {
                lblCountdown.Text = AppResources.CountDown;
                lblPayment.Text = AppResources.Payment;
                lblScanPayment.Text = AppResources.PleaseScanPayment;
            });
        }
    }
}