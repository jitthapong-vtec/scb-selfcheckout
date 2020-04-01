using SelfCheckout.Models;
using SelfCheckout.Services.Print;
using SelfCheckout.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Storage;
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
        public Task<string> GetSavedPrinterName()
        {
            var printerName = ApplicationData.Current.LocalSettings.Values["PrinterName"] as string;
            return Task.FromResult<string>(printerName);
        }

        public async Task<string> PickPrinterAsync()
        {
            DevicePicker devicePicker = new DevicePicker();
            devicePicker.Appearance.Title = "Printers";

            devicePicker.Filter.SupportedDeviceSelectors.Add("System.Devices.InterfaceClassGuid:=\"{0ecef634-6ef0-472a-8085-5ad023ecbccd}\"");

            var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
            GeneralTransform ge = Window.Current.Content.TransformToVisual(Window.Current.Content as UIElement);
            Rect rect = ge.TransformBounds(new Rect(pointerPosition.X, pointerPosition.Y, 0, 0));

            DeviceInformation deviceInfo = await devicePicker.PickSingleDeviceAsync(rect);
            return deviceInfo.Name;
        }

        public async Task PrintBitmapFromUrl(List<string> urls)
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("List_Invoices") == false)
            {
                ApplicationData.Current.LocalSettings.CreateContainer("List_Invoices", ApplicationDataCreateDisposition.Always);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Containers["List_Invoices"].Values.Clear();
            }

            for (var i = 0; i < urls.Count(); i++)
            {
                var url = urls[i];
                var rass = RandomAccessStreamReference.CreateFromUri(new Uri(url));
                using (var stream = await rass.OpenReadAsync())
                {
                    StorageFolder storageFolder = ApplicationData.Current.TemporaryFolder;

                    var fileBytes = new byte[stream.Size];
                    using (var reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);
                    }
                    var storageFile = await storageFolder.CreateFileAsync($"{Guid.NewGuid()}.png");
                    await FileIO.WriteBytesAsync(storageFile, fileBytes);
                    ApplicationData.Current.LocalSettings.Containers["List_Invoices"].Values[i.ToString()] = storageFile.Path;
                }
            }

            if (urls.Any())
            {
                if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
                {
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                }
            }
        }

        public Task SavePrinterName(string printerName)
        {
            ApplicationData.Current.LocalSettings.Values["PrinterName"] = printerName;
            return Task.FromResult(true);
        }
    }
}
