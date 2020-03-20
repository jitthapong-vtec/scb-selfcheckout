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
    public class DeviceStatusViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        ObservableCollection<DeviceStatus> _devices;

        int _totalOccupiedDevice;

        public DeviceStatusViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public ICommand SearchCommand => new DelegateCommand<string>(async (search) =>
        {
            await GetDeviceStatusAsync(search);
        });

        public ObservableCollection<DeviceStatus> Devices
        {
            get => _devices;
            set => SetProperty(ref _devices, value);
        }

        public int TotalOccupiedDevice
        {
            get => _totalOccupiedDevice;
            set => SetProperty(ref _totalOccupiedDevice, value);
        }

        async Task GetDeviceStatusAsync(string search = "")
        {
            try
            {
                IsBusy = true;

                var devices = await _selfCheckoutService.GetDeviceStatusAsync(search);
                Devices = devices.ToObservableCollection();

                TotalOccupiedDevice = devices.Where(d => d.SessionStatus.SessionCode == "START").Count();
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
