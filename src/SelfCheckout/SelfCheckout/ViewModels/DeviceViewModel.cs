using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        List<SimpleSelectedItem> _allDeviceInfoItems;
        ObservableCollection<SimpleSelectedItem> _tabs;
        ObservableCollection<SimpleSelectedItem> _deviceInfoItems;

        public DeviceViewModel()
        {
            Tabs = new ObservableCollection<SimpleSelectedItem>()
            {
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.General,
                    Arg1 = 1,
                    Selected = true
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.System,
                    Arg1 = 2
                }
            };

            _allDeviceInfoItems = new List<SimpleSelectedItem>()
            {
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.Name,
                    Text2 = "",
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.DateTime,
                    Text2 = DateTime.Now.ToString("dd/MM/yyyy HH:mm tt"),
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.FlightNo,
                    Text2 = "",
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.MobileNo,
                    Text2 = "",
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.Module,
                    Text2 = "",
                    Arg1 = 2
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.BranchNo,
                    Text2 = "",
                    Arg1 = 2
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.SubBranch,
                    Text2 = "",
                    Arg1 = 2
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.MachineNo,
                    Text2 = "",
                    Arg1 = 2
                },
            };

            RefreshDeviceInfo(1);
        }

        public ICommand TabSelectedCommand => new Command<SimpleSelectedItem>((item) =>
        {
            var seletedItem = Tabs.Where(t => t.Selected).FirstOrDefault();
            seletedItem.Selected = false;

            item.Selected = true;
            RefreshDeviceInfo(item.Arg1);
        });

        void RefreshDeviceInfo(object arg)
        {
            var filter = _allDeviceInfoItems.Where(d => (int)d.Arg1 == (int)arg).ToList();
            DeviceInfoItems = filter.ToObservableCollection();
        }

        public ObservableCollection<SimpleSelectedItem> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;
                RaisePropertyChanged(() => Tabs);
            }
        }

        public ObservableCollection<SimpleSelectedItem> DeviceInfoItems
        {
            get => _deviceInfoItems;
            set
            {
                _deviceInfoItems = value;
                RaisePropertyChanged(() => DeviceInfoItems);
            }
        }
    }
}
