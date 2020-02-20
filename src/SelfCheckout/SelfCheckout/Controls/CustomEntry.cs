using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SelfCheckout.Controls
{
    public class CustomEntry : Entry
    {
        public static BindableProperty UnfocusedCommandProperty =
            BindableProperty.Create("UnforcusedCommand", typeof(ICommand), typeof(CustomEntry), null,
                BindingMode.OneWay);

        public static BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(CustomEntry), null,
                BindingMode.OneWay);

        public ICommand UnfocusedCommand
        {
            get => (ICommand)GetValue(UnfocusedCommandProperty);
            set
            {
                SetValue(UnfocusedCommandProperty, value);
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

        public CustomEntry()
        {
            Unfocused += CustomEntry_Unfocused;
        }

        private void CustomEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (UnfocusedCommand.CanExecute(CommandParameter))
                UnfocusedCommand?.Execute(CommandParameter);
        }
    }
}
