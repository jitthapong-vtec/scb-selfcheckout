using Rg.Plugins.Popup.Pages;
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
    public partial class PromptPayQrDialog : PopupPage
    {
        public PromptPayQrDialog()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}