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
using System.Collections.ObjectModel;

namespace SelfCheckout.ViewModels.Base
{
    public abstract class ViewModelBase : BindableBase, IInitialize, IDestructible
    {
        ObservableCollection<Language> _languages;
        Language _languageSelected;

        string _pageTitle;
        bool _isBusy;
        bool _isRefreshing;
        bool _langShowing;

        public virtual Task OnTabSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        public virtual Task OnTabDeSelected(TabItem item)
        {
            return Task.FromResult(item);
        }

        protected void SetOrderImage(List<OrderDetail> orders)
        {
            Task.Run(async() =>
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
            });
        }

        public ICommand LanguageSelectionCommand => new Command<Language>((lang) =>
        {
            LanguageSelected = lang;
            LangShowing = false;
        });

        public ICommand LanguageTappedCommand => new Command(() =>
        {
            LangShowing = !LangShowing;
        });

        protected virtual Task OnLanguageChanged(Language lang)
        {
            return Task.FromResult(false);
        }

        protected virtual Task OnLanguageViewShowingChanged(bool isShowing)
        {
            return Task.FromResult(false);
        }

        public ObservableCollection<Language> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        public Language LanguageSelected
        {
            get => _languageSelected;
            set => SetProperty(ref _languageSelected, value, async () =>
            {
                var selfCheckoutService = Prism.PrismApplicationBase.Current.Container.Resolve(typeof(ISelfCheckoutService)) as ISelfCheckoutService;
                selfCheckoutService.CurrentLanguage = value;

                if (value.LangCode == "EN")
                    GlobalSettings.Instance.CountryCode = "en-US";
                else if (value.LangCode == "TH")
                    GlobalSettings.Instance.CountryCode = "th-TH";
                else if (value.LangCode == "ZH")
                    GlobalSettings.Instance.CountryCode = "zh-Hans";
                GlobalSettings.Instance.InitLanguage();
                await OnLanguageChanged(value);
            });
        }

        public bool LangShowing
        {
            get => _langShowing;
            set => SetProperty(ref _langShowing, value, () =>
            {
                OnLanguageViewShowingChanged(value);
            });
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
