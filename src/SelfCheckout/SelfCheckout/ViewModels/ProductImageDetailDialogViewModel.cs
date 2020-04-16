using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using SelfCheckout.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SelfCheckout.ViewModels
{
    public class ProductImageDetailDialogViewModel : PopupNavigationBase<INavigationParameters>
    {
        string _imageUrl;

        public ProductImageDetailDialogViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public ICommand CloseCommand => new DelegateCommand(async () =>
        {
            await GoBackAsync(null);
        });

        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            ImageUrl = parameters.GetValue<string>("ImageUrl");
        }
    }
}
