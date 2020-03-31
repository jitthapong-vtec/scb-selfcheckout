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
    public partial class PaymentMethodSelectionView : ContentView
    {
        public PaymentMethodSelectionView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MainViewModel>(this, ViewModelBase.MessageKey_LanguageChanged, (sender) =>
            {
                lblSelectPaymentMethod.Text = AppResources.SelectPaymentMethod;
            });
        }
    }
}