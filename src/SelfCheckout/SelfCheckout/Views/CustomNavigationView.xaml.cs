using SelfCheckout.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomNavigationView : NavigationPage
    {
        public CustomNavigationView()
        {
            InitializeComponent();
            Init();
        }

        public CustomNavigationView(Page root) : base(root)
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            Pushed += CustomNavigationView_Pushed;
            Popped += CustomNavigationView_Popped;
        }

        private void CustomNavigationView_Pushed(object sender, NavigationEventArgs e)
        {
            ((ViewModelBase)e.Page.BindingContext).NavigationPushed();
        }

        private void CustomNavigationView_Popped(object sender, NavigationEventArgs e)
        {
            ((ViewModelBase)CurrentPage.BindingContext).NavigationPoped();
        }
    }
}