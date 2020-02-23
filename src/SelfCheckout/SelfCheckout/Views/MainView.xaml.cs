using SelfCheckout.Controls;
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
    public partial class MainView : DensoScannerPage
    {
        MainViewModel _viewModel;

        public MainView()
        {
            InitializeComponent();

            _viewModel = BindingContext as MainViewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void TappedGrid_Tapped(object sender, object e)
        {
            if (_viewModel.CurrentView is ShoppingCartView)
            {
                var shoppingCartViewModel = _viewModel.CurrentView.BindingContext as ShoppingCartViewModel;
                if (!shoppingCartViewModel.IsFirstSelect)
                    FireScanEvent();
            }
        }
    }
}