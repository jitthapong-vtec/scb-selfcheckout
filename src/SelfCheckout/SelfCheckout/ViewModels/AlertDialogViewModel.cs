using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.Services.Register;
using SelfCheckout.Services.SaleEngine;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class AlertDialogViewModel : PopupNavigationBase<INavigationParameters>
    {
        string _title;
        string _message;
        string _okButtonText;

        public AlertDialogViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string OkButtonText
        {
            get => _okButtonText;
            set => SetProperty(ref _okButtonText, value);
        }

        public ICommand OkCommand => new DelegateCommand(async() =>
        {
            await GoBackAsync(null);
        });

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Title = parameters.GetValue<string>("Title");
            Message = parameters.GetValue<string>("Message");
            OkButtonText = parameters.GetValue<string>("OkButtonText");
        }
    }
}
