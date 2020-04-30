using Prism.Services.Dialogs;
using SelfCheckout.Models;
using SelfCheckout.Resources;
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

        string _labelName;
        string _labelDateTime;
        string _labelFlightNo;
        string _labelMobileNo;

        public DeviceViewModel(ISaleEngineService saleEngineService,
            IRegisterService registerService)
        {
            _saleEngineService = saleEngineService;
            _registerService = registerService;

            RefreshLanguage();
        }

        public void RefreshLanguage()
        {
            LabelName = AppResources.Name;
            LabelDateTime = AppResources.DateTime;
            LabelFlightNo = AppResources.FlightNo;
            LabelMobileNo = AppResources.MobileNo;
        }

        public string LabelName
        {
            get => _labelName;
            set => SetProperty(ref _labelName, value);
        }

        public string LabelDateTime
        {
            get => _labelDateTime;
            set => SetProperty(ref _labelDateTime, value);
        }

        public string LabelFlightNo
        {
            get => _labelFlightNo;
            set => SetProperty(ref _labelFlightNo, value);
        }

        public string LabelMobileNo
        {
            get => _labelMobileNo;
            set => SetProperty(ref _labelMobileNo, value);
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

        public string MobileNo
        {
            get => CustomerData?.Person?.ListContact.FirstOrDefault()?.ContactValue;
        }
    }
}
