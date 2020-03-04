using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
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

        bool _isAuthorized;
        bool _logoutButtonVisible;

        public DeviceViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : base(navigatinService, dialogService, selfCheckoutService, saleEngineService, registerService)
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
                    Text2 = CustomerData?.Person?.EnglishName,
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
                    Text2 = CustomerData?.Person?.FlightCode,
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.MobileNo,
                    Text2 = CustomerData?.Person?.ListContact.FirstOrDefault()?.ContactValue,
                    Arg1 = 1
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.Module,
                    Text2 = AppConfig?.Module,
                    Arg1 = 2
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.BranchNo,
                    Text2 = AppConfig?.BranchNo,
                    Arg1 = 2
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.SubBranch,
                    Text2 = AppConfig?.SubBranch,
                    Arg1 = 2
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.MachineNo,
                    Text2 = "-",
                    Arg1 = 2
                },
            };
        }

        public ICommand TabSelectedCommand => new Command<SimpleSelectedItem>(async (item) =>
        {
            if ((int)item.Arg1 == 2 && !IsAuthorized)
            {
                DialogService.ShowDialog("AuthorizeDialog", null, (dialogResult) =>
                {
                    if (dialogResult.Parameters.GetValue<bool>("IsConfirm"))
                        IsAuthorized = true;
                });
            }

            var seletedItem = Tabs.Where(t => t.Selected).FirstOrDefault();
            seletedItem.Selected = false;

            item.Selected = true;
            LogoutButtonVisible = (int)item.Arg1 == 2 ? true : false;
            RefreshDeviceInfo(item.Arg1);
        });

        public override Task OnTabSelected(TabItem item)
        {
            TabSelectedCommand.Execute(Tabs.FirstOrDefault());
            return base.OnTabSelected(item);
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            IsAuthorized = false;
            return base.OnTabDeSelected(item);
        }

        void RefreshDeviceInfo(object arg)
        {
            var filter = _allDeviceInfoItems.Where(d => (int)d.Arg1 == (int)arg).ToList();
            DeviceInfoItems = filter.ToObservableCollection();
        }

        public ObservableCollection<SimpleSelectedItem> Tabs
        {
            get => _tabs;
            set => SetProperty(ref _tabs, value);
        }

        public ObservableCollection<SimpleSelectedItem> DeviceInfoItems
        {
            get => _deviceInfoItems;
            set => SetProperty(ref _deviceInfoItems, value);
        }

        public bool IsAuthorized
        {
            get => _isAuthorized;
            set => SetProperty(ref _isAuthorized, value);
        }

        public bool LogoutButtonVisible
        {
            get => _logoutButtonVisible;
            set => SetProperty(ref _logoutButtonVisible, value);
        }
    }
}
