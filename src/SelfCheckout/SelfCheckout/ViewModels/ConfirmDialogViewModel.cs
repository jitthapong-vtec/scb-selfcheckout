using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class ConfirmDialogViewModel : PopupNavigationBase<bool>
    {
        bool __okAsRedButton;

        string _title;
        string _message;
        string _okButtonText;
        string _cancelButtonText;

        public ConfirmDialogViewModel(INavigationService navigationService) : base(navigationService)
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

        public string CancelButtonText
        {
            get => _cancelButtonText;
            set => SetProperty(ref _cancelButtonText, value);
        }

        public bool OkAsRedButton {
            get => __okAsRedButton;
            set => SetProperty(ref __okAsRedButton, value);
        }

        public ICommand YesCommand => new DelegateCommand(async() =>
        {
            await GoBackAsync(true);
        });

        public ICommand NoCommand => new DelegateCommand(async() =>
        {
            await GoBackAsync(false);
        });

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Title = parameters.GetValue<string>("Title");
            Message = parameters.GetValue<string>("Message");
            OkButtonText = parameters.GetValue<string>("OkButtonText");
            CancelButtonText = parameters.GetValue<string>("CancelButtonText");
            OkAsRedButton = parameters.GetValue<bool>("OkAsRedButton");
        }
    }
}
