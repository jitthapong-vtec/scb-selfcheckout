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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        public Func<Task> GoBackToRootAsync;

        ISaleEngineService _saleEngineService;
        ISelfCheckoutService _selfCheckoutService;
        IRegisterService _registerService;

        public DeviceViewModel(IDialogService dialogService,
            ISelfCheckoutService selfCheckoutService, ISaleEngineService saleEngineService,
            IRegisterService registerService) : base(dialogService)
        {
            _saleEngineService = saleEngineService;
            _selfCheckoutService = selfCheckoutService;
            _registerService = registerService;

            //var appConfig = selfCheckoutService.AppConfig;
            //CustomerData = registerService.CustomerData;
            //var loginData = saleEngineService.LoginData;

            //DeviceInfoItems = new ObservableCollection<SimpleSelectedItem>()
            //{
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.Name,
            //        Text2 = customerData?.Person?.EnglishName,
            //        Arg1 = 1
            //    },
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.DateTime,
            //        Text2 = DateTime.Now.ToString("dd/MM/yyyy HH:mm tt"),
            //        Arg1 = 1
            //    },
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.FlightNo,
            //        Text2 = customerData?.Person?.FlightCode,
            //        Arg1 = 1
            //    },
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.MobileNo,
            //        Text2 = customerData?.Person?.ListContact.FirstOrDefault()?.ContactValue,
            //        Arg1 = 1
            //    },
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.Module,
            //        Text2 = appConfig?.Module,
            //        Arg1 = 2
            //    },
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.BranchNo,
            //        Text2 = appConfig?.BranchNo,
            //        Arg1 = 2
            //    },
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.SubBranch,
            //        Text2 = appConfig?.SubBranch,
            //        Arg1 = 2
            //    },
            //    new SimpleSelectedItem()
            //    {
            //        Text1 = AppResources.MachineNo,
            //        Text2 = loginData.UserInfo.MachineEnv.MachineNo,
            //        Arg1 = 2
            //    },
            //};
        }

        public ICommand LogoutCommand => new DelegateCommand(async () =>
        {
            try
            {
                var result = await DialogService.ConfirmAsync(AppResources.Logout, AppResources.ConfirmLogout, AppResources.Yes, AppResources.No);
                if (result)
                {
                    await _saleEngineService.LogoutAsync();
                    await GoBackToRootAsync();
                }
            }
            catch { }
        });

        public override Task OnTabSelected(TabItem item)
        {
            return base.OnTabSelected(item);
        }

        public override Task OnTabDeSelected(TabItem item)
        {
            return base.OnTabDeSelected(item);
        }

        public LoginData LoginData
        {
            get => _saleEngineService.LoginData;
        }

        public CustomerData CustomerData
        {
            get => _registerService.CustomerData;
        }

        public string NowDateTime
        {
            get => DateTime.Now.ToString("dd/MM/yyyy HH:mm tt");
        }

        public string MobileNo {
            get => CustomerData?.Person?.ListContact.FirstOrDefault()?.ContactValue;
        }
    }
}
