using SelfCheckout.Controls;
using SelfCheckout.Exceptions;
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
                {
                    try
                    {
                        FireScanEvent();
                    }
                    catch (DensoScannerException ex)
                    {
                        _viewModel.DialogService.ShowAlertAsync(AppResources.Opps, ex.Message, AppResources.Close);

                        //var shoppingCart = _viewModel.Tabs.Where(t => t.TabId == 3).FirstOrDefault();
                        //Task.Run(() => (shoppingCart.Page.BindingContext as ShoppingCartViewModel).TestAddOrder());
                    }
                }
            }
        }
    }
}