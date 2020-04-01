using System;
using System.Collections.Generic;
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
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("List_Invoices"))
            {
                var printerName = ApplicationData.Current.LocalSettings.Values["PrinterName"] as string;
                for (var i = 0; i< ApplicationData.Current.LocalSettings.Containers["List_Invoices"].Values.Count; i++)
                {
                    var fileToPrint = ApplicationData.Current.LocalSettings.Containers["List_Invoices"].Values[i.ToString()] as string;
                    Image img = Image.FromFile(fileToPrint);

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
