using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class OrderInvoiceGroup : List<OrderDetail>
    {
        public OrderInvoiceGroup(List<OrderDetail> orderDetails) : base(orderDetails)
        {
        }

        public string OrderNo { get; set; }
        public string CurrencyCode { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDateTime { get; set; }
        public string PassportNo { get; set; }
        public string PaymentType { get; set; }
        public string CustomerName { get; set; }
        public string ShoppingCardNo { get; set; }
        public double? TotalQty { get; set; }
        public double? SubTotal { get; set; }
        public double? TotalNet { get; set; }
        public double? TotalDiscount { get; set; }
    }
}
