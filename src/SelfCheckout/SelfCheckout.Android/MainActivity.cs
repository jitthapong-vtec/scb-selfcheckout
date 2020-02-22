using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acr.UserDialogs;
using FFImageLoading;
using Android.Support.V4.App;
using Android;
using Com.Densowave.Bhtsdk.Barcode;
using System.Collections.Generic;

namespace SelfCheckout.Droid
{
    [Activity(Label = "Self Checkout", Icon = "@mipmap/ic_launcher", Theme = "@style/Theme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration()
            {
                DiskCacheDuration = TimeSpan.FromSeconds(20)
            });
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            UserDialogs.Init(this);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            ActivityCompat.RequestPermissions(this, new string[]{
                    Manifest.Permission.WriteExternalStorage,
                    Manifest.Permission.Camera}, 1212);
            base.OnStart();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}