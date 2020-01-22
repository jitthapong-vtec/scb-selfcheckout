using SelfCheckout.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        ObservableCollection<TabItem> _tabs;

        public MainViewModel()
        {
            _tabs = new ObservableCollection<TabItem>();
            _tabs.Add(new TabItem()
            {
                PageTitle = "Promotion",
                Icon = "ic_coupon.png"
            });
            _tabs.Add(new TabItem()
            {
                PageTitle = "",
                Icon = "ic_barcode.png"
            });
            _tabs.Add(new TabItem()
            {
                PageTitle = "My Order",
                Icon = "ic_myorder.png"
            });
            _tabs.Add(new TabItem()
            {
                PageTitle = "Profile",
                Icon = "ic_user.png"
            });
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }
        
        public ObservableCollection<TabItem> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;
                RaisePropertyChanged(() => Tabs);
            }
        }
    }
}
