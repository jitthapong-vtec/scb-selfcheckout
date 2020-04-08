using Prism.Commands;
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

namespace SelfCheckout.ViewModels
{
    public class SessionHistoryViewModel : SessionOrderViewModelBase
    {
        List<DeviceStatus> _allSessionHistories;
        ObservableCollection<DeviceStatus> _sessionHistories;
        ObservableCollection<SimpleSelectedItem> _filterTypes;

        DateTime? _filterDate;

        string _filterSessionKey;
        string _filterMachineNo;

        public SessionHistoryViewModel(IDialogService dialogService, 
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService, IRegisterService registerService) : 
            base(dialogService, selfCheckoutService, saleEngineService, registerService)
        {
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

        public DateTime MaxDate
        {
            get => DateTime.Today;
        }

        public string FilterSessionKey
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
            ShowSessionOrder(sess);
        });

        public ICommand ChangeFilterTypeCommand => new DelegateCommand<SimpleSelectedItem>((filterType) =>
        {
            var selectedItem = FilterTypes.Where(f => f.Selected).FirstOrDefault();
            selectedItem.Selected = false;

            filterType.Selected = true;

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
            FilterTypes.Where(f => f.Selected).FirstOrDefault().Selected = false;
            FilterTypes.Where(f => (int)f.Arg1 == 1).FirstOrDefault().Selected = true;

            await LoadSessionHistoryAsync();
        });

        protected override async Task SessionCloseCallback()
        {
            await LoadSessionHistoryAsync();
        }

        async Task LoadSessionHistoryAsync()
        {
            try
            {
                IsBusy = true;
                long sessionKey = 0;
                try
                {
                    sessionKey = Convert.ToInt64(FilterSessionKey);
                }
                catch { }
                _allSessionHistories = await SelfCheckoutService.GetSessionHistory(FilterDate, sessionKey, FilterMachineNo);
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
