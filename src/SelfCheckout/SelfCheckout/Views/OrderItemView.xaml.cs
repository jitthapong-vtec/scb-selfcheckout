using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SelfCheckout.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderItemView : ContentView
    {
        public static readonly BindableProperty SelectionCommandProperty =
            BindableProperty.Create("SelectionCommand", typeof(ICommand), typeof(OrderItemView), null, BindingMode.OneWay);

        public static readonly BindableProperty ChangeQtyCommandProperty =
            BindableProperty.Create("ChangeQtyCommand", typeof(ICommand), typeof(OrderItemView), null, BindingMode.OneWay);

        public static BindableProperty DeleteCommandProperty =
            BindableProperty.Create("DeleteCommand", typeof(ICommand), typeof(OrderItemView), null, BindingMode.OneWay);

        public static BindableProperty ShowDetailCommandProperty =
            BindableProperty.Create("ShowDetailCommand", typeof(ICommand), typeof(OrderItemView), null, BindingMode.OneWay);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(OrderItemView), null, BindingMode.OneWay);

        public static readonly BindableProperty IsEnableEntryOrderQtyProperty =
            BindableProperty.Create("IsEnableEntryOrderQty", typeof(bool), typeof(OrderItemView), true, propertyChanged: IsEnableEntryOrderQtyPropertyChanged);

        public static readonly BindableProperty IsEnableChkOrderProperty =
            BindableProperty.Create("IsEnableChkOrder", typeof(bool), typeof(OrderItemView), true, propertyChanged: IsEnableChkOrderPropertyChanged);

        private static void IsEnableChkOrderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                (bindable as OrderItemView).chkOrder.IsEnabled = (bool)newValue;
            }
            catch { }
        }

        static void IsEnableEntryOrderQtyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                (bindable as OrderItemView).entryOrderQty.IsReadOnly = !(bool)newValue;
            }
            catch { }
        }

        public bool IsEnableEntryOrderQty
        {
            get => (bool)GetValue(IsEnableEntryOrderQtyProperty);
            set => SetValue(IsEnableEntryOrderQtyProperty, value);
        }

        public bool IsEnableChkOrder
        {
            get => (bool)GetValue(IsEnableChkOrderProperty);
            set => SetValue(IsEnableChkOrderProperty, value);
        }

        public ICommand SelectionCommand
        {
            get => (ICommand)GetValue(SelectionCommandProperty);
            set
            {
                SetValue(SelectionCommandProperty, value);
            }
        }

        public ICommand ChangeQtyCommand
        {
            get => (ICommand)GetValue(ChangeQtyCommandProperty);
            set
            {
                SetValue(ChangeQtyCommandProperty, value);
            }
        }

        public ICommand ShowDetailCommand
        {
            get => (ICommand)GetValue(ShowDetailCommandProperty);
            set
            {
                SetValue(ShowDetailCommandProperty, value);
            }
        }

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set
            {
                SetValue(DeleteCommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public OrderItemView()
        {
            InitializeComponent();
        }

        public void Delete_Tapped(object sender, object e)
        {
            DeleteCommand?.Execute(CommandParameter);
        }

        public void CheckBox_Tapped(object sender, object e)
        {
            SelectionCommand?.Execute(CommandParameter);
        }

        private void Entry_Qty_Unfocused(object sender, FocusEventArgs e)
        {
            ChangeQtyCommand?.Execute(CommandParameter);
        }

        private void ShowDetail_Tapped(object sender, object e)
        {
            ShowDetailCommand?.Execute(CommandParameter);
        }
    }
}