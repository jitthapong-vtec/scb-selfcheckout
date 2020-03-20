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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class SessionHistoryViewModel : ViewModelBase
    {
        ISelfCheckoutService _selfCheckoutService;

        ObservableCollection<DeviceStatus> _sessionHistories;

        public SessionHistoryViewModel(INavigationService navigatinService, IDialogService dialogService, ISelfCheckoutService selfCheckoutService) : base(navigatinService, dialogService)
        {
            _selfCheckoutService = selfCheckoutService;
        }

        public ObservableCollection<DeviceStatus> SessionHistories
        {
            get => _sessionHistories;
            set => SetProperty(ref _sessionHistories, value);
        }

        public ICommand FilterCommand => new DelegateCommand(async() =>
        {
            await LoadSessionHistoryAsync();
        });

        async Task LoadSessionHistoryAsync()
        {
            try
            {
                IsBusy = true;
                var histories = await _selfCheckoutService.GetSessionHistory(null, 0, "");
                SessionHistories = histories.ToObservableCollection();
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
