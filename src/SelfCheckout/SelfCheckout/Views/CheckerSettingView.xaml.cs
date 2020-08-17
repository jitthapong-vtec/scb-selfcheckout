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
    public partial class CheckerSettingView : ContentPage
    {
        public CheckerSettingView()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            ((SettingViewModelBase)BindingContext)?.BackToRootCommand.Execute(null);
            return base.OnBackButtonPressed();
        }
    }
}