using DLToolkit.Forms.Controls;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using SelfCheckout.Services.Payment;
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
            FlowListView.Init();

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
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<AlertDialog, AlertDialogViewModel>("AlertDialog");
            containerRegistry.RegisterForNavigation<ConfirmDialog, ConfirmDialogViewModel>("ConfirmDialog");
            containerRegistry.RegisterForNavigation<AuthorizationDialog, AuthorizationDialogViewModel>("AuthorizeDialog");
            containerRegistry.RegisterForNavigation<CustomerShoppingCardConfirmDialog, CustomerShoppingCardConfirmDialogViewModel>("CustomerCardConfirmDialog");
            containerRegistry.RegisterForNavigation<ShoppingCardInputDialog, ShoppingCardInputDialogViewModel>("ShoppingCardInputDialog");
            containerRegistry.RegisterForNavigation<SessionOrderDialog, SessionOrderDialogViewModel>("SessionOrderDialog");
            containerRegistry.RegisterForNavigation<PromptPayQrDialog, PromptPayQrDialogViewModel>("PromptPayQrDialog");
            containerRegistry.RegisterForNavigation<ProductImageDetailDialog, ProductImageDetailDialogViewModel>("ProductImageDetailDialog");
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<CheckerSettingView, CheckerSettingViewModel>();
            containerRegistry.RegisterForNavigation<LandingView, LandingViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<BorrowView, BorrowViewModel>();
            containerRegistry.RegisterForNavigation<CheckerMainView, CheckerMainViewModel>();
            containerRegistry.RegisterForNavigation<OrderDetailView, OrderDetailViewModel>();
            containerRegistry.RegisterForNavigation<CameraScannerView, CameraScannerViewModel>();

            containerRegistry.RegisterSingleton<ISerializeService, JsonSerializeService>();
            containerRegistry.RegisterSingleton<ISaleEngineService, SaleEngineService>();
            containerRegistry.RegisterSingleton<ISelfCheckoutService, SelfCheckoutService>();
            containerRegistry.RegisterSingleton<IRegisterService, RegisterService>();
            containerRegistry.RegisterSingleton<IPimCoreService, PimCoreService>();
            containerRegistry.RegisterSingleton<IPaymentService, PaymentService>();
        }
    }
}
