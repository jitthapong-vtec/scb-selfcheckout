using SelfCheckout.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Prism.Commands;
using SelfCheckout.Services.SelfCheckout;
using SelfCheckout.Services.SaleEngine;
using System.Collections.Generic;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ViewModelBase : BindableBase, IInitialize, IDestructible
    {
        public IDialogService DialogService { get; private set; }

        string _pageTitle;
        bool _isBusy;
        bool _isRefreshing;

        public ViewModelBase(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        public virtual Task OnTabSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public virtual Task OnTabDeSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        protected async Task SetOrderImage(List<OrderDetail> orders)
        {
            var selfCheckoutService = Prism.PrismApplicationBase.Current.Container.Resolve(typeof(ISelfCheckoutService)) as ISelfCheckoutService;
            if (IsShowArticleImage)
            {
                try
                {
                    foreach (var order in orders)
                    {
                        var result = await selfCheckoutService.GetArticleImageAsync(order.ItemDetail.Item.Code);
                        order.ImageUrl = result.ImageUrl;
                    }
                }
                catch { }
            }
        }

        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public bool IsShowArticleImage
        {
            get
            {
                return (Prism.PrismApplicationBase.Current.Container.Resolve(typeof(ISelfCheckoutService)) as ISelfCheckoutService).AppConfig.ShowArticleImage;
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public bool IsBusy
        {
            get => _isBusy;

            set => SetProperty(ref _isBusy, value);
        }

        public string Version { get => VersionTracking.CurrentVersion; }

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void Destroy()
        {
        }
    }
}
