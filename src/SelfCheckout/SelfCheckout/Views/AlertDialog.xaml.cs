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
    public partial class AlertDialog : PopupPage
    {
        public AlertDialog()
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = false;
        }
    }
}