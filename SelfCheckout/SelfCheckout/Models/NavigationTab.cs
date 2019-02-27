using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace SelfCheckout.Models
{
    public class NavigationTab : INotifyPropertyChanged
    {
        int _tabId;
        string _pageTitle;
        string _icon;
        ContentView _page;
        bool _selected;
        int _badgeCount;

        public int TabId {
            get => _tabId;
            set => _tabId = value;
        }

        public ContentView Page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value;
                NotifyPropertyChanged();
            }
        }

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                _pageTitle = value;
                NotifyPropertyChanged();
            }
        }

        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                NotifyPropertyChanged();
            }
        }

        public int BadgeCount {
            get
            {
                return _badgeCount;
            }
            set
            {
                _badgeCount = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
