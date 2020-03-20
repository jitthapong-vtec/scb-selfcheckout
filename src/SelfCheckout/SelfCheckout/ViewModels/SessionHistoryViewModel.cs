using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Extensions;
using SelfCheckout.Models;
using SelfCheckout.Resources;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class SessionHistoryViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        List<DeviceStatus> _allSessionHistories;
        ObservableCollection<DeviceStatus> _sessionHistories;
        ObservableCollection<SimpleSelectedItem> _filterTypes;

        DateTime? _filterDate;
        int _filterSessionKey;
        string _filterMachineNo;

        public SessionHistoryViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;

            FilterTypes = new ObservableCollection<SimpleSelectedItem>() {
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.All,
                    Arg1 = 1,
                    Selected = true
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.Occupied,
                    Arg1 = 2
                },
                new SimpleSelectedItem()
                {
                    Text1 = AppResources.Finished,
                    Arg1 = 3
                }
            };
        }

        public ObservableCollection<DeviceStatus> SessionHistories
        {
            get => _sessionHistories;
            set => SetProperty(ref _sessionHistories, value);
        }

        public ObservableCollection<SimpleSelectedItem> FilterTypes
        {
            get => _filterTypes;
            set => SetProperty(ref _filterTypes, value);
        }

        public DateTime? FilterDate
        {
            get => _filterDate;
            set => SetProperty(ref _filterDate, value);
        }

        public int FilterSessionKey
        {
            get => _filterSessionKey;
            set => SetProperty(ref _filterSessionKey, value);
        }

        public string FilterMachineNo
        {
            get => _filterMachineNo;
            set => SetProperty(ref _filterMachineNo, value);
        }

        public ICommand ShowOrderDetailCommand => new DelegateCommand<DeviceStatus>((sess) =>
        {
            var parameters = new DialogParameters()
            {
                {"SessionKey", sess.SessionKey },
                {"ShoppingCard", sess.ShoppingCard }
            };
            try
            {
                DialogService.ShowDialogAsync("SessionOrderDialog", parameters);
            }
            catch { }
        });

        public ICommand ChangeFilterTypeCommand => new DelegateCommand<SimpleSelectedItem>((filterType) =>
        {
            filterType.Selected = true;

            var selectedItem = FilterTypes.Where(f => f.Selected).FirstOrDefault();
            selectedItem.Selected = false;

            try
            {
                switch ((int)filterType.Arg1)
                {
                    case 1:
                        SessionHistories = _allSessionHistories.ToObservableCollection();
                        break;
                    case 2:
                        SessionHistories = _allSessionHistories.Where(s => s.SessionStatus.SessionCode == "START").ToObservableCollection();
                        break;
                    case 3:
                        SessionHistories = _allSessionHistories.Where(s => s.SessionStatus.SessionCode == "END").ToObservableCollection();
                        break;
                }
            }
            catch { }
        });

        public ICommand FilterCommand => new DelegateCommand(async () =>
        {
            await LoadSessionHistoryAsync();
        });

        async Task LoadSessionHistoryAsync()
        {
            try
            {
                IsBusy = true;
                _allSessionHistories = await _selfCheckoutService.GetSessionHistory(FilterDate, FilterSessionKey, FilterMachineNo);
                SessionHistories = _allSessionHistories.ToObservableCollection();
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(AppResources.Opps, ex.Message, AppResources.Close);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
