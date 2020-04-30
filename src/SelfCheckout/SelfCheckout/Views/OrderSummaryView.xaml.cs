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
    public partial class OrderSummaryView : ContentView
    {
        public OrderSummaryView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MainViewModel>(this, "LanguageChange", (sender) =>
            {
                lblOrderSummary.Text = AppResources.OrderSummary;
                lblInvoice.Text = AppResources.Invoices;
                lblSubTotal.Text = AppResources.SubTotal;
                lblUnit.Text = AppResources.Units;
                lblDiscount.Text = AppResources.Discount;
                lblOrderTotal.Text = AppResources.OrderTotal;
            });
        }
    }
}