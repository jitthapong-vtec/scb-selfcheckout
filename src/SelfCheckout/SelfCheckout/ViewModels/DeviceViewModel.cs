using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.ViewModels.Base;
using System;
using System.Linq;

namespace SelfCheckout.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        ISaleEngineService _saleEngineService;
        IRegisterService _registerService;

        public DeviceViewModel(IDialogService dialogService, ISaleEngineService saleEngineService,
            IRegisterService registerService) : base(dialogService)
        {
            _saleEngineService = saleEngineService;
            _registerService = registerService;
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
