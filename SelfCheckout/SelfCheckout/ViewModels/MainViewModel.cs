using SelfCheckout.Models;
using SelfCheckout.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SelfCheckout.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        ObservableCollection<NavigationTab> _tabs;

        public MainViewModel()
        {
            _tabs = new ObservableCollection<NavigationTab>();
            _tabs.Add(new NavigationTab()
            {
                PageTitle = "Promotion",
                Icon = "ic_coupon.png"
            });
            _tabs.Add(new NavigationTab()
            {
                PageTitle = "My Order",
                Icon = "ic_myorder.png"
            });
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }
        
        public ObservableCollection<NavigationTab> Tabs
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
