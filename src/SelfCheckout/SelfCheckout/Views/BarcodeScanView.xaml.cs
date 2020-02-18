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
	public partial class BarcodeScanView : ContentPage
	{
		public BarcodeScanView ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            scanView.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            scanView.IsScanning = false;
            ((BarcodeScanViewModel)BindingContext).SetResult(null);
        }
    }
}