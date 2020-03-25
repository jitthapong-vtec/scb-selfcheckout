using SelfCheckout.Resources;
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
    public partial class SummaryView : ContentView
    {
        public SummaryView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MainViewModel>(this, "LanguageChanged", (sender) =>
            {
                if (sender.SummaryVisible)
                    sender.SummaryVisible = false;

                if (string.IsNullOrEmpty(sender.CouponCode))
                    lblScanCoupon.Text = AppResources.ScanCoupon;

                sender.CheckoutButtonText = AppResources.Checkout;

                lblOrderTotal.Text = AppResources.OrderTotal;
                lblDiscount.Text = AppResources.Discount;
                lblSelectPaymentMethod.Text = AppResources.SelectPaymentMethod;
                lblSubTotal.Text = AppResources.SubTotal;
                lblSummaryOrder.Text = AppResources.OrderSummary;
                lblUnit.Text = AppResources.Units;
            });
        }
    }
}