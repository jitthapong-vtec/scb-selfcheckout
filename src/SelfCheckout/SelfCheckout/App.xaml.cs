using SelfCheckout.Services.Navigation;
using SelfCheckout.ViewModels.Base;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SelfCheckout
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            GlobalSettings.Instance.InitLanguage();

            if (Device.RuntimePlatform == Device.UWP)
            {
                InitNavigation();
            }
        }

        public Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        protected override async void OnStart()
        {
            if (Device.RuntimePlatform != Device.UWP)
            {
                await InitNavigation();
            }
            base.OnResume();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
