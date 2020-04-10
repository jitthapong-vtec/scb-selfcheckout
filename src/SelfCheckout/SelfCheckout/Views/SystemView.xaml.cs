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
    public partial class SystemView : ContentView
    {
        public SystemView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MainViewModel>(this, "LanguageChange", (sender) =>
            {
                lblSystem.Text = AppResources.System;
                lblModule.Text = AppResources.Module;
                lblBranchNo.Text = AppResources.BranchNo;
                lblSubBranch.Text = AppResources.SubBranch;
                lblMachineNo.Text = AppResources.MachineNo;
            });
        }
    }
}