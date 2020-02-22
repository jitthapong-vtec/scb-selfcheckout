using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Densowave.Bhtsdk.Barcode;
using SelfCheckout.Controls;
using SelfCheckout.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(DensoScannerPage), typeof(DensoScannerPageRenderer))]
namespace SelfCheckout.Droid.Renderers
{
    public class DensoScannerPageRenderer : PageRenderer, BarcodeManager_.IBarcodeManagerListener_, BarcodeScanner_.IBarcodeDataListener_
    {
        BarcodeScanner_ mBarcodeScanner;

        public DensoScannerPageRenderer(Context context) : base(context)
        {
            try
            {
                BarcodeManager_.Create(Context, this);
            }
            catch { }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            (Element as DensoScannerPage).ScanButtonClick += DensoScannerPageRenderer_ScannButtonClick;
        }

        private void DensoScannerPageRenderer_ScannButtonClick(object sender, EventArgs e)
        {
            try
            {
                mBarcodeScanner.PressSoftwareTrigger(true);
                Task.Delay(1000).Wait();
                mBarcodeScanner.PressSoftwareTrigger(false);
            }
            catch(Exception ex)
            {
                mBarcodeScanner.PressSoftwareTrigger(false);
            }
        }

        public void OnBarcodeDataReceived(BarcodeDataReceivedEvent_ dataReceivedEvent)
        {
            mBarcodeScanner.PressSoftwareTrigger(false);
            IList<BarcodeDataReceivedEvent_.BarcodeData_> listBarcodeData = dataReceivedEvent.BarcodeData;

            (Element as DensoScannerPage).ScanCommand?.Execute(listBarcodeData.FirstOrDefault().Data);
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
            catch { }
        }
    }
}