using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using SelfCheckout.Droid.Services;
using SelfCheckout.Services;
using Xamarin.Forms;

[assembly:Dependency(typeof(NativeApiService))]
namespace SelfCheckout.Droid.Services
{
    public class NativeApiService : INativeApiService
    {
        public void ExitApp()
        {
            try
            {
                CrossCurrentActivity.Current.Activity.Finish();
            }
            catch { }
        }
    }
}