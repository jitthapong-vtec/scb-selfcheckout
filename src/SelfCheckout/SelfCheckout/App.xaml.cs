using Prism;
using Prism.Ioc;
using SelfCheckout.Services.PimCore;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.Services.Serializer;
using SelfCheckout.ViewModels;
using SelfCheckout.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SelfCheckout
{
    public partial class App
    {
        public App() : this(null)
        {
        }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            Device.SetFlags(new[] {
                "CarouselView_Experimental",
                "IndicatorView_Experimental",
                "SwipeView_Experimental"
            });

            GlobalSettings.Instance.InitLanguage();

            await NavigationService.NavigateAsync("NavigationPage/LandingView");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DialogView, DialogViewModel>("CommonDialog");
            containerRegistry.RegisterDialog<ConfirmDialogView, ConfirmDialogViewModel>("ConfirmDialog");
            containerRegistry.RegisterDialog<AuthorizationDialog, AuthorizationDialogViewModel>("AuthorizeDialog");
            containerRegistry.RegisterDialog<CustomerCartConfirmDialog, CustomerCartConfirmDialogViewModel>("CustomerCardConfirmDialog");
            containerRegistry.RegisterDialog<BarcodeScanView, BarcodeScanViewModel>("BarcodeScanDialog");
            containerRegistry.RegisterDialog<ShoppingCardInputDialog, ShoppingCardInputDialogViewModel>("ShoppingCardInputDialog");

            containerRegistry.Register<ShoppingCartViewModel>();
            containerRegistry.Register<DeviceViewModel>();
            containerRegistry.Register<OrderViewModel>();
            containerRegistry.Register<ProfileViewModel>();
            containerRegistry.Register<HomeViewModel>();
            containerRegistry.Register<CheckerPackingViewModel>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<LandingView, LandingViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<BorrowView, BorrowViewModel>();
            containerRegistry.RegisterForNavigation<CheckerMainView, CheckerMainViewModel>();

            containerRegistry.RegisterSingleton<ISerializeService, JsonSerializeService>();
            containerRegistry.RegisterSingleton<ISaleEngineService, SaleEngineService>();
            containerRegistry.RegisterSingleton<ISelfCheckoutService, SelfCheckoutService>();
            containerRegistry.RegisterSingleton<IRegisterService, RegisterService>();
            containerRegistry.RegisterSingleton<IPimCoreService, PimCoreService>();
        }
    }
}
