using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Services
{
    public class Utils
    {
        public static string TryDecodeKioskShoppingCard(string barcode)
        {
            try
            {
                var definition = new { S = "", C = "" };
                var qrFromKiosk = JsonConvert.DeserializeAnonymousType(barcode, definition);
                barcode = qrFromKiosk.S;
            }
            catch { }
            return barcode;
        }
    }
}
