using SelfCheckout.Controls;
using SelfCheckout.Exceptions;
using SelfCheckout.Extensions;
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
    public partial class MainView : DensoScannerPage
    {
        public MainView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<ViewModelBase>(this, "RequestHWScanner", (sender) =>
            {
                FireScanEvent();
            });
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void TappedGrid_Tapped(object sender, object e)
        {
            var mainViewModel = (BindingContext as MainViewModel);
            if (mainViewModel.CurrentView is ShoppingCartView)
            {
                var shoppingCartViewModel = mainViewModel.CurrentView.BindingContext as ShoppingCartViewModel;
                if (!shoppingCartViewModel.IsFirstSelect)
                {
                    try
                    {
                        FireScanEvent();
                    }
                    catch (DensoScannerException ex)
                    {
                    }
                }
                shoppingCartViewModel.IsFirstSelect = false;
            }
        }
    }
}