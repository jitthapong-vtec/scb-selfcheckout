﻿using SelfCheckout.Models;
using SelfCheckout.Services.Print;
using SelfCheckout.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(PrintService))]
namespace SelfCheckout.UWP.Services
{
    public class PrintService : IPrintService
    {
        PosPrinter _printer;
        ClaimedPosPrinter _claimedPrinter;

        public async Task<Printer> FindAndClaimPrinter()
        {
            DevicePicker devicePicker = new DevicePicker();
            devicePicker.Filter.SupportedDeviceSelectors.Add(PosPrinter.GetDeviceSelector());

            var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;

            GeneralTransform ge = Window.Current.Content.TransformToVisual(Window.Current.Content as UIElement);
            Rect rect = ge.TransformBounds(new Rect(pointerPosition.X, pointerPosition.Y, 0, 0));

            DeviceInformation deviceInfo = await devicePicker.PickSingleDeviceAsync(rect);
            _printer = await PosPrinter.FromIdAsync(deviceInfo.Id);

            return new Printer() { PrinterDeviceId = deviceInfo.Id, PrinterName = deviceInfo.Name };
        }

        public async Task PrintBitmapFromUrl(string url)
        {
            await ClaimAndEnablePrinter();

            var rass = RandomAccessStreamReference.CreateFromUri(new Uri(url));
            using (var stream = await rass.OpenReadAsync())
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                using (var encoderStream = new InMemoryRandomAccessStream())
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(encoderStream, decoder);
                    var newWidth = Convert.ToUInt32(_claimedPrinter.Receipt.PageSize.Width);
                    var newHeight = decoder.PixelHeight;
                    encoder.BitmapTransform.ScaledHeight = newHeight;
                    encoder.BitmapTransform.ScaledWidth = newWidth;

                    await encoder.FlushAsync();

                    byte[] pixels = new byte[newWidth * newHeight * 4];

                    await encoderStream.ReadAsync(pixels.AsBuffer(), (uint)pixels.Length, InputStreamOptions.None);
                    var final = await BitmapDecoder.CreateAsync(encoderStream);
                    var bitmapFrame = await final.GetFrameAsync(0);

                    _claimedPrinter.Receipt.IsLetterQuality = true;

                    ReceiptPrintJob job = _claimedPrinter.Receipt.CreateJob();
                    job.PrintBitmap(bitmapFrame, PosPrinterAlignment.Center);
                    string feedString = "";
                    for (uint n = 0; n < _claimedPrinter.Receipt.LinesToPaperCut; n++)
                    {
                        feedString += "\n";
                    }
                    job.Print(feedString);
                    if (_printer.Capabilities.Receipt.CanCutPaper)
                    {
                        job.CutPaper();
                    }
                    await job.ExecuteAsync();
                    ReleasePrinter();
                }
            }
        }

        private async Task ClaimAndEnablePrinter()
        {
            var printerId = Preferences.Get("PrinterId", "");
            if (string.IsNullOrEmpty(printerId))
                throw new Exception("Not found config printer");

            try
            {
                _printer = await PosPrinter.FromIdAsync(printerId);
                _claimedPrinter = await _printer.ClaimPrinterAsync();
                await _claimedPrinter.EnableAsync();
            }
            catch
            {
                ReleasePrinter();
            }
        }

        private void ReleasePrinter()
        {
            try
            {
                _claimedPrinter.Dispose();
                _claimedPrinter = null;

                _printer.Dispose();
                _printer = null;
            }
            catch { }
        }
    }
}
