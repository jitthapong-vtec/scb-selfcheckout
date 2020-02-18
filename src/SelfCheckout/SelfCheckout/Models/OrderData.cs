using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SelfCheckout.Models
{
    public class OrderData
    {
        [JsonProperty("Guid")]
        public string Guid { get; set; }

        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty("Consolidate")]
        public bool Consolidate { get; set; }

        [JsonProperty("HeaderAttributes")]
        public List<Attribute> HeaderAttributes { get; set; }

        [JsonProperty("BillingQuantities")]
        public List<BillingQuantity> BillingQuantities { get; set; }

        [JsonProperty("CustomerDetail")]
        public CustomerDetail CustomerDetail { get; set; }

        [JsonProperty("OrderDetails")]
        public List<OrderDetail> OrderDetails { get; set; }

        [JsonProperty("BillingAmount")]
        public IngAmount BillingAmount { get; set; }

        [JsonProperty("BasketBillingAmount")]
        public IngAmount BasketBillingAmount { get; set; }

        [JsonProperty("TotalBillingAmount")]
        public TotalBillingAmount TotalBillingAmount { get; set; }

        [JsonProperty("RemainingAmount")]
        public IngAmount RemainingAmount { get; set; }

        [JsonProperty("ChangeAmount")]
        public ChangeAmount ChangeAmount { get; set; }

        [JsonProperty("OrderInvoices")]
        public List<OrderInvoice> OrderInvoices { get; set; }

        [JsonProperty("OrderPayments")]
        public List<OrderPayment> OrderPayments { get; set; }

        [JsonProperty("OrderPaymentsHistory")]
        public List<object> OrderPaymentsHistory { get; set; }

        [JsonProperty("Voucher")]
        public List<object> Voucher { get; set; }

        [JsonProperty("PaymentSession")]
        public PaymentSession PaymentSession { get; set; }

        [JsonProperty("PaymentTransaction")]
        public Transaction PaymentTransaction { get; set; }

        [JsonProperty("isRequireSignature")]
        public bool IsRequireSignature { get; set; }

        [JsonProperty("OrderSignatures")]
        public List<object> OrderSignatures { get; set; }

        [JsonProperty("isRefund")]
        public bool IsRefund { get; set; }

        [JsonProperty("isFinish")]
        public bool IsFinish { get; set; }

        [JsonProperty("isChange")]
        public bool IsChange { get; set; }

        [JsonProperty("isExtConsolidate")]
        public bool IsExtConsolidate { get; set; }

        [JsonProperty("isNotAllowSMC")]
        public bool IsNotAllowSmc { get; set; }

        [JsonProperty("isBirthday")]
        public bool IsBirthday { get; set; }

        [JsonProperty("UseBirthday")]
        public bool UseBirthday { get; set; }

        [JsonProperty("isCheckOut")]
        public bool IsCheckOut { get; set; }

        [JsonProperty("listSubsidize")]
        public object ListSubsidize { get; set; }

        [JsonProperty("GWPDetails")]
        public List<object> GwpDetails { get; set; }
    }

    public class IngAmount
    {
        [JsonProperty("TotalAmount")]
        public DiscountAmount TotalAmount { get; set; }

        [JsonProperty("DiscountAmount")]
        public DiscountAmount DiscountAmount { get; set; }

        [JsonProperty("ValueAdjusts")]
        public List<object> ValueAdjusts { get; set; }

        [JsonProperty("PercentDiscount")]
        public double? PercentDiscount { get; set; }

        [JsonProperty("VATAmount")]
        public DiscountAmount VatAmount { get; set; }

        [JsonProperty("NetAmount")]
        public DiscountAmount NetAmount { get; set; }
    }

    public class DiscountAmount
    {
        [JsonProperty("CurrCode")]
        public BaseCurrCode CurrCode { get; set; }

        [JsonProperty("CurrRate")]
        public double? CurrRate { get; set; }

        [JsonProperty("CurrAmt")]
        public double? CurrAmt { get; set; }

        [JsonProperty("CurrAmtForCal")]
        public double? CurrAmtForCal { get; set; }

        [JsonProperty("BaseCurrCode")]
        public BaseCurrCode BaseCurrCode { get; set; }

        [JsonProperty("BaseCurrRate")]
        public double? BaseCurrRate { get; set; }

        [JsonProperty("BaseCurrAmt")]
        public double? BaseCurrAmt { get; set; }

        [JsonProperty("BaseCurrAmtForCal")]
        public double? BaseCurrAmtForCal { get; set; }
    }

    public class BaseCurrCode
    {
        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Desc")]
        public string Desc { get; set; }
    }

    public class BillingQuantity
    {
        [JsonProperty("Quantity")]
        public double? Quantity { get; set; }

        [JsonProperty("UOM")]
        public string Uom { get; set; }
    }

    public class ChangeAmount
    {
        [JsonProperty("totalChange")]
        public double? TotalChange { get; set; }

        [JsonProperty("totalLocalChange")]
        public double? TotalLocalChange { get; set; }

        [JsonProperty("totalamount")]
        public double? Totalamount { get; set; }

        [JsonProperty("CurrCode")]
        public BaseCurrCode CurrCode { get; set; }

        [JsonProperty("CurrRate")]
        public double? CurrRate { get; set; }

        [JsonProperty("CurrAmt")]
        public double? CurrAmt { get; set; }

        [JsonProperty("CurrAmtForCal")]
        public double? CurrAmtForCal { get; set; }

        [JsonProperty("BaseCurrCode")]
        public BaseCurrCode BaseCurrCode { get; set; }

        [JsonProperty("BaseCurrRate")]
        public double? BaseCurrRate { get; set; }

        [JsonProperty("BaseCurrAmt")]
        public double? BaseCurrAmt { get; set; }

        [JsonProperty("BaseCurrAmtForCal")]
        public double? BaseCurrAmtForCal { get; set; }
    }

    public class CustomerDetail
    {
        [JsonProperty("CustomerType")]
        public BaseCurrCode CustomerType { get; set; }

        [JsonProperty("CustomerName")]
        public string CustomerName { get; set; }

        [JsonProperty("Nationality")]
        public BaseCurrCode Nationality { get; set; }

        [JsonProperty("CommissionGroup")]
        public CommissionGroup CommissionGroup { get; set; }

        [JsonProperty("CustomerAttributes")]
        public List<Attribute> CustomerAttributes { get; set; }
    }

    public class CommissionGroup
    {
        [JsonProperty("Group")]
        public BaseCurrCode Group { get; set; }

        [JsonProperty("Source")]
        public object Source { get; set; }
    }

    public class Attribute
    {
        [JsonProperty("Group")]
        public string Group { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Value")]
        public object Value { get; set; }

        [JsonProperty("ValueOfString")]
        public string ValueOfString { get; set; }

        [JsonProperty("ValueOfDecimal")]
        public double? ValueOfDecimal { get; set; }

        [JsonProperty("ValueOfDateTime")]
        public DateTime? ValueOfDateTime { get; set; }

        [JsonProperty("ValueOfTime")]
        public DateTime? ValueOfTime { get; set; }

        [JsonProperty("DataType")]
        public object DataType { get; set; }
    }

    public class OrderDetail : INotifyPropertyChanged
    {
        bool _isSelected;

        [JsonProperty("Guid")]
        public string Guid { get; set; }

        [JsonProperty("LineNo")]
        public int LineNo { get; set; }

        [JsonProperty("BarCode")]
        public object BarCode { get; set; }

        [JsonProperty("BillingQuantity")]
        public BillingQuantity BillingQuantity { get; set; }

        [JsonProperty("BillingAmount")]
        public IngAmount BillingAmount { get; set; }

        [JsonProperty("Promotion")]
        public BaseCurrCode Promotion { get; set; }

        [JsonProperty("SubOrderType")]
        public BaseCurrCode SubOrderType { get; set; }

        [JsonProperty("IsFreeze")]
        public bool IsFreeze { get; set; }

        [JsonProperty("IsLockDiscount")]
        public bool IsLockDiscount { get; set; }

        [JsonProperty("IsConsolidate")]
        public bool IsConsolidate { get; set; }

        [JsonProperty("IsExtConsolidate")]
        public bool IsExtConsolidate { get; set; }

        [JsonProperty("IsCancel")]
        public bool IsCancel { get; set; }

        [JsonProperty("IsBinManagement")]
        public bool IsBinManagement { get; set; }

        [JsonProperty("IsBasket")]
        public bool IsBasket { get; set; }

        [JsonProperty("ItemDetail")]
        public ItemDetail ItemDetail { get; set; }

        [JsonProperty("Seller")]
        public Cashier Seller { get; set; }

        [JsonProperty("lineItemDatabase")]
        public int LineItemDatabase { get; set; }

        [JsonProperty("DetailAttributes")]
        public object DetailAttributes { get; set; }

        public bool IsSelected {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ItemDetail
    {
        [JsonProperty("MC")]
        public List<BaseCurrCode> Mc { get; set; }

        [JsonProperty("Cate")]
        public BaseCurrCode Cate { get; set; }

        [JsonProperty("Section")]
        public BaseCurrCode Section { get; set; }

        [JsonProperty("SubSection")]
        public BaseCurrCode SubSection { get; set; }

        [JsonProperty("ItemType")]
        public BaseCurrCode ItemType { get; set; }

        [JsonProperty("Item")]
        public BaseCurrCode Item { get; set; }

        [JsonProperty("Detail")]
        public string Detail { get; set; }

        [JsonProperty("BaseUnitPrice")]
        public double? BaseUnitPrice { get; set; }

        [JsonProperty("UnitPrice")]
        public double? UnitPrice { get; set; }

        [JsonProperty("Brand")]
        public BaseCurrCode Brand { get; set; }

        [JsonProperty("MaxPercentDiscount")]
        public double? MaxPercentDiscount { get; set; }

        [JsonProperty("VAT")]
        public BaseCurrCode Vat { get; set; }

        [JsonProperty("VATRate")]
        public double? VatRate { get; set; }

        [JsonProperty("ItemAttributes")]
        public List<Attribute> ItemAttributes { get; set; }
    }

    public class Cashier
    {
        [JsonProperty("MachineNo")]
        public string MachineNo { get; set; }

        [JsonProperty("MachineTaxNo")]
        public string MachineTaxNo { get; set; }

        [JsonProperty("EmployeeNo")]
        public string EmployeeNo { get; set; }

        [JsonProperty("IPNo")]
        public string IpNo { get; set; }

        [JsonProperty("MachineDateTime")]
        public DateTime MachineDateTime { get; set; }
    }

    public class OrderInvoice
    {
        [JsonProperty("Guid")]
        public string Guid { get; set; }

        [JsonProperty("InvoiceNo")]
        public int InvoiceNo { get; set; }

        [JsonProperty("TaxInvoiceType")]
        public BaseCurrCode TaxInvoiceType { get; set; }

        [JsonProperty("TaxInvoiceNo")]
        public object TaxInvoiceNo { get; set; }

        [JsonProperty("TaxAbbNo")]
        public string TaxAbbNo { get; set; }

        [JsonProperty("ClaimcheckNo")]
        public string ClaimcheckNo { get; set; }

        [JsonProperty("RunningNo")]
        public object RunningNo { get; set; }

        [JsonProperty("RecNo")]
        public int RecNo { get; set; }

        [JsonProperty("PackingNo")]
        public string PackingNo { get; set; }

        [JsonProperty("PickupCode")]
        public string PickupCode { get; set; }

        [JsonProperty("HangingNo")]
        public string HangingNo { get; set; }

        [JsonProperty("SubOrderType")]
        public BaseCurrCode SubOrderType { get; set; }

        [JsonProperty("Amount")]
        public object Amount { get; set; }

        [JsonProperty("Cashier")]
        public Cashier Cashier { get; set; }

        [JsonProperty("StepInvoice")]
        public object StepInvoice { get; set; }

        [JsonProperty("RunnerBy")]
        public object RunnerBy { get; set; }

        [JsonProperty("RunnerDateTime")]
        public DateTime RunnerDateTime { get; set; }

        [JsonProperty("Box")]
        public object Box { get; set; }

        [JsonProperty("BoxBy")]
        public object BoxBy { get; set; }

        [JsonProperty("BoxDateTime")]
        public DateTime BoxDateTime { get; set; }

        [JsonProperty("Trip")]
        public object Trip { get; set; }

        [JsonProperty("Truck")]
        public object Truck { get; set; }

        [JsonProperty("TruckBy")]
        public object TruckBy { get; set; }

        [JsonProperty("TruckDateTime")]
        public DateTime TruckDateTime { get; set; }

        [JsonProperty("BillingQuantity")]
        public BillingQuantity BillingQuantity { get; set; }

        [JsonProperty("BillingAmount")]
        public IngAmount BillingAmount { get; set; }

        [JsonProperty("IssuedType")]
        public string IssuedType { get; set; }

        [JsonProperty("IssuedDetail")]
        public string IssuedDetail { get; set; }

        [JsonProperty("OrderDetails")]
        public List<OrderDetail> OrderDetails { get; set; }

        [JsonProperty("OrderPayments")]
        public List<OrderPayment> OrderPayments { get; set; }
    }

    public class OrderPayment
    {
        [JsonProperty("Guid")]
        public string Guid { get; set; }

        [JsonProperty("LineNo")]
        public int LineNo { get; set; }

        [JsonProperty("PaymentIcon")]
        public object PaymentIcon { get; set; }

        [JsonProperty("GatewayId")]
        public long GatewayId { get; set; }

        [JsonProperty("PaymentCode")]
        public string PaymentCode { get; set; }

        [JsonProperty("PaymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("RefNo")]
        public object RefNo { get; set; }

        [JsonProperty("CardHolderName")]
        public string CardHolderName { get; set; }

        [JsonProperty("ApproveCode")]
        public object ApproveCode { get; set; }

        [JsonProperty("BankOfEDC")]
        public string BankOfEdc { get; set; }

        [JsonProperty("IssuerID")]
        public object IssuerId { get; set; }

        [JsonProperty("AliBarcode")]
        public object AliBarcode { get; set; }

        [JsonProperty("AliMerchantID")]
        public object AliMerchantId { get; set; }

        [JsonProperty("AliTransID")]
        public object AliTransId { get; set; }

        [JsonProperty("AlipayCancel")]
        public bool AlipayCancel { get; set; }

        [JsonProperty("WalletBarcode")]
        public object WalletBarcode { get; set; }

        [JsonProperty("WalletMerchantID")]
        public object WalletMerchantId { get; set; }

        [JsonProperty("WalletTransID")]
        public object WalletTransId { get; set; }

        [JsonProperty("isDCC")]
        public bool IsDcc { get; set; }

        [JsonProperty("isCheckVoucher")]
        public bool IsCheckVoucher { get; set; }

        [JsonProperty("isFixAmount")]
        public bool IsFixAmount { get; set; }

        [JsonProperty("isNotAllowSMC")]
        public bool IsNotAllowSmc { get; set; }

        [JsonProperty("isComplese")]
        public object IsComplese { get; set; }

        [JsonProperty("URLService")]
        public object UrlService { get; set; }

        [JsonProperty("PaymentAmounts")]
        public DiscountAmount PaymentAmounts { get; set; }

        [JsonProperty("Transaction")]
        public Transaction Transaction { get; set; }

        [JsonProperty("isVoucher")]
        public bool IsVoucher { get; set; }

        [JsonProperty("isSubsidize")]
        public bool IsSubsidize { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("PaymentSessionKey")]
        public int PaymentSessionKey { get; set; }

        [JsonProperty("PartnerTransID")]
        public object PartnerTransId { get; set; }

        [JsonProperty("listVoucher")]
        public object ListVoucher { get; set; }
    }

    public class Transaction
    {
        [JsonProperty("TransactionId")]
        public int? TransactionId { get; set; }

        [JsonProperty("TransactionGroup")]
        public int? TransactionGroup { get; set; }

        [JsonProperty("TransactionType")]
        public int? TransactionType { get; set; }

        [JsonProperty("PartnerGroup")]
        public object PartnerGroup { get; set; }

        [JsonProperty("PartnerType")]
        public int? PartnerType { get; set; }

        [JsonProperty("PartnerId")]
        public int PartnerId { get; set; }

        [JsonProperty("GatewaySessionKey")]
        public object GatewaySessionKey { get; set; }

        [JsonProperty("GatewayBuyerId")]
        public string GatewayBuyerId { get; set; }

        [JsonProperty("GatewayBarcode")]
        public string GatewayBarcode { get; set; }

        [JsonProperty("Cashier")]
        public Cashier Cashier { get; set; }

        [JsonProperty("MerchantId")]
        public object MerchantId { get; set; }

        [JsonProperty("PartnerTransId")]
        public string PartnerTransId { get; set; }

        [JsonProperty("LastStatus")]
        public int? LastStatus { get; set; }

        [JsonProperty("CurrentStatus")]
        public int CurrentStatus { get; set; }

        [JsonProperty("CreateDateTime")]
        public DateTime CreateDateTime { get; set; }

        [JsonProperty("ModifiedDateTime")]
        public DateTime? ModifiedDateTime { get; set; }

        [JsonProperty("PrintSlip")]
        public bool PrintSlip { get; set; }

        [JsonProperty("PrintSlipText")]
        public string PrintSlipText { get; set; }

        [JsonProperty("Movements")]
        public List<PaymentTransactionMovement> Movements { get; set; }

        [JsonProperty("BillingAmount")]
        public IngAmount BillingAmount { get; set; }
    }

    public class PaymentTransactionMovement
    {
        [JsonProperty("TransactionMovementType")]
        public int TransactionMovementType { get; set; }

        [JsonProperty("Amount")]
        public double? Amount { get; set; }

        [JsonProperty("Currency")]
        public BaseCurrCode Currency { get; set; }

        [JsonProperty("CurrencyRate")]
        public double? CurrencyRate { get; set; }

        [JsonProperty("Description")]
        public object Description { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("DateTime")]
        public DateTime DateTime { get; set; }
    }

    public class PaymentSession
    {
        [JsonProperty("SessionId")]
        public int SessionId { get; set; }

        [JsonProperty("PaidGuid")]
        public string PaidGuid { get; set; }

        [JsonProperty("LastStatus")]
        public object LastStatus { get; set; }

        [JsonProperty("CurrentStatus")]
        public int CurrentStatus { get; set; }

        [JsonProperty("CreateDateTime")]
        public DateTime CreateDateTime { get; set; }

        [JsonProperty("ModifiedDateTime")]
        public object ModifiedDateTime { get; set; }

        [JsonProperty("Movements")]
        public List<PaymentSessionMovement> Movements { get; set; }
    }

    public class PaymentSessionMovement
    {
        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("DateTime")]
        public DateTime DateTime { get; set; }
    }

    public class TotalBillingAmount
    {
        [JsonProperty("DiscountSpecial")]
        public DiscountAmount DiscountSpecial { get; set; }

        [JsonProperty("PercentDiscountSpecial")]
        public double? PercentDiscountSpecial { get; set; }

        [JsonProperty("CurrentValueAdjust")]
        public object CurrentValueAdjust { get; set; }

        [JsonProperty("TotalSubsidize")]
        public DiscountAmount TotalSubsidize { get; set; }

        [JsonProperty("TotalPaid")]
        public DiscountAmount TotalPaid { get; set; }

        [JsonProperty("TotalNetPay")]
        public DiscountAmount TotalNetPay { get; set; }

        [JsonProperty("TotalAmount")]
        public DiscountAmount TotalAmount { get; set; }

        [JsonProperty("DiscountAmount")]
        public DiscountAmount DiscountAmount { get; set; }

        [JsonProperty("ValueAdjusts")]
        public List<object> ValueAdjusts { get; set; }

        [JsonProperty("PercentDiscount")]
        public double? PercentDiscount { get; set; }

        [JsonProperty("VATAmount")]
        public DiscountAmount VatAmount { get; set; }

        [JsonProperty("NetAmount")]
        public DiscountAmount NetAmount { get; set; }
    }
}
