using SelfCheckout.Resources;
using SelfCheckout.ViewModels;
using SelfCheckout.ViewModels.Base;
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

            MessagingCenter.Subscribe<MainViewModel>(this, ViewModelBase.MessageKey_LanguageChanged, (sender) =>
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