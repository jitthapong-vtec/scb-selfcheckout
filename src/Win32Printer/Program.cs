using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Win32Printer
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("FileToPrint"))
            {
                var printerName = ApplicationData.Current.LocalSettings.Values["PrinterName"] as string;

                Image img = Image.FromFile(ApplicationData.Current.LocalSettings.Values["FileToPrint"] as string);

                PrintDocument doc = new PrintDocument();
                PrintController printController = new StandardPrintController();
                doc.PrintController = printController;

                if (!string.IsNullOrEmpty(printerName))
                    doc.PrinterSettings.PrinterName = printerName;
                doc.PrintPage += new PrintPageEventHandler((sender, e) =>
                {
                    //img = ResizeImage(img, e.Graphics.VisibleClipBounds.Size);
                    e.Graphics.DrawImage(img, Point.Empty);
                    //e.HasMorePages = true;
                });
                doc.EndPrint += new PrintEventHandler((sender, e) =>
                {
                    resetEvent.Set();
                });
                doc.Print();
            }
            resetEvent.WaitOne();
        }

        public static Image ResizeImage(Image img, SizeF targetSize)
        {
            float scale = Math.Min(targetSize.Width / img.Width, targetSize.Height / img.Height);
            Size newSize = new Size((int)(img.Width * scale) - 1, (int)(img.Height * scale) - 1);
            return new Bitmap(img, newSize);
        }
    }
}
