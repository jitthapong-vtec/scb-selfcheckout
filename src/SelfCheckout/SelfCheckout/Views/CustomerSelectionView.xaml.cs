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
    public partial class CustomerSelectionView : ContentView
    {
        public CustomerSelectionView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MainViewModel>(this, "LanguageChange", (sender) =>
            {
                lblFilterCustomer.Text = AppResources.FilterCustomer;
            });
        }
    }
}