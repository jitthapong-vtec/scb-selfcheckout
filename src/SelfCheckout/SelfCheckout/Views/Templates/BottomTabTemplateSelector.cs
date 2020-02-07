using SelfCheckout.Models;
using SelfCheckout.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SelfCheckout.Views.Templates
{
    public class BottomTabTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommonTapTemplate { get; set; }
        public DataTemplate BarcodeTapTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var tabItem = item as TabItem;
            return tabItem?.TabType == 1 ? BarcodeTapTemplate : CommonTapTemplate;
        }
    }
}
