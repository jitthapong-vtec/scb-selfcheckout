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
    public partial class CheckerPackingView : ContentView
    {
        public CheckerPackingView()
        {
            InitializeComponent();
        }

        private void SetEntryQrCode()
        {
            if (!string.IsNullOrWhiteSpace(entrySessionKey.Text))
                hidenEntry.Text = entrySessionKey.Text;
        }

        private void entrySessionKey_Completed(object sender, EventArgs e)
        {
            SetEntryQrCode();
        }

        private void TappedGrid_Tapped(object sender, object e)
        {
            SetEntryQrCode();
        }
    }
}