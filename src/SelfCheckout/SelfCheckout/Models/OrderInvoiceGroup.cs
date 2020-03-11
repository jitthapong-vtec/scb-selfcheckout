using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class OrderInvoiceGroup : List<OrderDetail>
    {
        public int ViewType { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDateTime { get; set; }
        public string PassportNo { get; set; }
        public string PaymentType { get; set; }
        public string CustomerName { get; set; }
        public string ShoppingCardNo { get; set; }
        public decimal TotalNet { get; set; }
    }
}
