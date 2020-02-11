using SelfCheckout.Controls;
using SelfCheckout.ViewModels.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace SelfCheckout.Models
{
    public class TabItem : INotifyPropertyChanged
    {
        int _tabId;
        string _title;
        string _tabText;
        string _icon;
        ContentView _page;
        bool _selected;
        int _badgeCount;
        bool _badgeVisible;
        int _tabType;

        public int TabId
        {
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

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        public string TabText
        {
            get
            {
                return _tabText;
            }
            set
            {
                _tabText = value;
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
                if (value)
                    (_page.BindingContext as ViewModelBase)?.OnTabSelected(this);
                else
                    (_page.BindingContext as ViewModelBase)?.OnTabDeSelected(this);
            }
        }

        public int BadgeCount
        {
            get
            {
                return _badgeCount;
            }
            set
            {
                _badgeCount = value;
                NotifyPropertyChanged();
                BadgeVisible = value > 0;
            }
        }

        public bool BadgeVisible
        {
            get => _badgeVisible;
            set
            {
                _badgeVisible = value;
                NotifyPropertyChanged();
            }
        }

        public int TabType
        {
            get => _tabType;
            set
            {
                _tabType = value;
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
