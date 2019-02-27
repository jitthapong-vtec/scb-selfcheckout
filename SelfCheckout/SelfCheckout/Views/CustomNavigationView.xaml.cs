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
            Popped += (s, e) =>
            {
                InvokePopback(e.Page);
            };
        }

        void InvokePopback(Page page)
        {
            ((ViewModelBase)page.BindingContext).OnPopbackAsync();
        }
    }
}