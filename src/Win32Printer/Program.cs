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
                    doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    doc.PrintController = printController;

                    if (!string.IsNullOrEmpty(printerName))
                        doc.PrinterSettings.PrinterName = printerName;
                    doc.PrintPage += new PrintPageEventHandler((sender, e) =>
                    {
                        var targetSize = e.Graphics.VisibleClipBounds.Size;
                        var scale = Math.Min(targetSize.Width / img.Width, targetSize.Height / img.Height);
                        var newWidth = (int)(img.Width * scale) - 1;
                        var newHeight = (int)(img.Height * scale) - 1;
                        var rect = new Rectangle(0, 0, newWidth, newHeight);
                        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        e.Graphics.DrawImage(img, rect);
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
    }
}
