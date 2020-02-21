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
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, BarcodeManager_.IBarcodeManagerListener_, BarcodeScanner_.IBarcodeDataListener_
    {
        BarcodeScanner_ mBarcodeScanner;

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

            global::Xamarin.Forms.Forms.SetFlags("SwipeView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            ActivityCompat.RequestPermissions(this, new string[]{
                    Manifest.Permission.WriteExternalStorage,
                    Manifest.Permission.Camera}, 1212);

            try
            {
                BarcodeManager_.Create(this, this);
            }
            catch { }

            base.OnStart();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            try
            {
                mBarcodeScanner.Destroy();
            }
            catch { }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;
        }

        public void OnBarcodeManagerCreated(BarcodeManager_ barcodeManager)
        {
            try
            {
                IList<BarcodeScanner_> listScanner = barcodeManager.BarcodeScanners;
                if (listScanner.Count > 0)
                {
                    mBarcodeScanner = listScanner[0];
                    mBarcodeScanner.AddDataListener(this);
                    mBarcodeScanner.Claim();
                }
            }
            catch (BarcodeException_ e)
            {
            }
        }
    }
}