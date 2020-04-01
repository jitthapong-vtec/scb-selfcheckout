using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Print
{
    public interface IPrintService
    {
        Task<string> PickPrinterAsync();

        Task SavePrinterName(string printerName);

        Task<string> GetSavedPrinterName();

        Task PrintBitmapFromUrl(List<string> urls);
    }
}
