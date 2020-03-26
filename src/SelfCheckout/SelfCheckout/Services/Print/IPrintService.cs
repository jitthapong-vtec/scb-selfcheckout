using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Print
{
    public interface IPrintService
    {
        Task<Printer> FindAndClaimPrinter();

        Task PrintBitmapFromUrl(string url);
    }
}
