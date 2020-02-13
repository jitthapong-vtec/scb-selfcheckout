using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class CustomerData
    {
        [JsonProperty("person")]
        public Person Person { get; set; }

        [JsonProperty("agentCode")]
        public string AgentCode { get; set; }

        [JsonProperty("subAgentCode")]
        public string SubAgentCode { get; set; }

        [JsonProperty("tour")]
        public Tour Tour { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("isFound")]
        public bool IsFound { get; set; }

        [JsonProperty("pathURL")]
        public string PathUrl { get; set; }

        [JsonProperty("stringToPrint")]
        public string StringToPrint { get; set; }

        [JsonProperty("pathURLMemberCard")]
        public string PathUrlMemberCard { get; set; }

        [JsonProperty("listCoupon")]
        public List<ListCoupon> ListCoupon { get; set; }

        [JsonProperty("kpPromotion")]
        public Promotion KpPromotion { get; set; }

        [JsonProperty("isMember")]
        public bool IsMember { get; set; }

        [JsonProperty("isCumulative")]
        public bool IsCumulative { get; set; }
    }

    public class Promotion
    {
        [JsonProperty("isMember")]
        public bool IsMember { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("qrPromotion")]
        public string QrPromotion { get; set; }

        [JsonProperty("listPromotion")]
        public List<ListPromotion> ListPromotion { get; set; }
    }

    public class ListPromotion
    {
        [JsonProperty("pathPromotion")]
        public string PathPromotion { get; set; }

        [JsonProperty("promotionTitle")]
        public string PromotionTitle { get; set; }

        [JsonProperty("promotionDesc")]
        public string PromotionDesc { get; set; }

        [JsonProperty("promotionBenefit")]
        public string PromotionBenefit { get; set; }

        [JsonProperty("promotionKey")]
        public long PromotionKey { get; set; }

        [JsonProperty("QRCode")]
        public string QrCode { get; set; }

        [JsonProperty("JsonQrCode")]
        public string JsonQrCode { get; set; }

        [JsonProperty("CouponType")]
        public string CouponType { get; set; }

        [JsonProperty("isMember")]
        public bool IsMember { get; set; }

        [JsonProperty("expiredDate")]
        public string ExpiredDate { get; set; }

        [JsonProperty("benefitID")]
        public long BenefitId { get; set; }

        [JsonProperty("card_id")]
        public string CardId { get; set; }

        [JsonProperty("CouponBenefitCode")]
        public string CouponBenefitCode { get; set; }

        [JsonProperty("CouponLimitCode")]
        public string CouponLimitCode { get; set; }

        [JsonProperty("benifitQRCode")]
        public string BenifitQrCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("benefitSeq")]
        public long BenefitSeq { get; set; }

        [JsonProperty("isCashCard")]
        public bool IsCashCard { get; set; }
    }

    public class ListCoupon
    {
        [JsonProperty("couponCode")]
        public string CouponCode { get; set; }

        [JsonProperty("couponDetail")]
        public string CouponDetail { get; set; }

        [JsonProperty("couponQRCode")]
        public string CouponQrCode { get; set; }

        [JsonProperty("couponEndPoint")]
        public string CouponEndPoint { get; set; }
    }

    public class Person
    {
        [JsonProperty("runningNo")]
        public long RunningNo { get; set; }

        [JsonProperty("customerTypeCode")]
        public string CustomerTypeCode { get; set; }

        [JsonProperty("customerTypeDetail")]
        public string CustomerTypeDetail { get; set; }

        [JsonProperty("englishName")]
        public string EnglishName { get; set; }

        [JsonProperty("nativeName")]
        public string NativeName { get; set; }

        [JsonProperty("passportNo")]
        public string PassportNo { get; set; }

        [JsonProperty("airlineCode")]
        public string AirlineCode { get; set; }

        [JsonProperty("flightCode")]
        public string FlightCode { get; set; }

        [JsonProperty("flightDate")]
        public DateTimeOffset FlightDate { get; set; }

        [JsonProperty("flightTime")]
        public string FlightTime { get; set; }

        [JsonProperty("flightRoute")]
        public string FlightRoute { get; set; }

        [JsonProperty("flightRouteDetail")]
        public string FlightRouteDetail { get; set; }

        [JsonProperty("flightpickup")]
        public string Flightpickup { get; set; }

        [JsonProperty("provinceCode")]
        public string ProvinceCode { get; set; }

        [JsonProperty("cityCode")]
        public string CityCode { get; set; }

        [JsonProperty("listContact")]
        public List<ListContact> ListContact { get; set; }

        [JsonProperty("listIdentity")]
        public List<ListIdentity> ListIdentity { get; set; }

        [JsonProperty("singleDiscount")]
        public List<ListIdentity> SingleDiscount { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("order_status")]
        public string OrderStatus { get; set; }

        [JsonProperty("order_date")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("fast_register")]
        public bool FastRegister { get; set; }

        [JsonProperty("isActivate")]
        public bool IsActivate { get; set; }

        [JsonProperty("PreRegister")]
        public bool PreRegister { get; set; }

        [JsonProperty("orderNo")]
        public long OrderNo { get; set; }

        [JsonProperty("promotion")]
        public Promotion Promotion { get; set; }

        [JsonProperty("mob")]
        public long Mob { get; set; }

        [JsonProperty("yob")]
        public long Yob { get; set; }
    }

    public class ListContact
    {
        [JsonProperty("contactType")]
        public string ContactType { get; set; }

        [JsonProperty("contactValue")]
        public string ContactValue { get; set; }
    }

    public class ListIdentity
    {
        [JsonProperty("IdentityType")]
        public string IdentityType { get; set; }

        [JsonProperty("IdentityValue")]
        public string IdentityValue { get; set; }
    }

    public class Tour
    {
        [JsonProperty("tourCode")]
        public string TourCode { get; set; }

        [JsonProperty("tourDate")]
        public DateTime TourDate { get; set; }

        [JsonProperty("tourTime")]
        public string TourTime { get; set; }

        [JsonProperty("discountSource")]
        public string DiscountSource { get; set; }

        [JsonProperty("airlineCode")]
        public string AirlineCode { get; set; }

        [JsonProperty("flightCode")]
        public string FlightCode { get; set; }

        [JsonProperty("flightDate")]
        public DateTime FlightDate { get; set; }

        [JsonProperty("flightTime")]
        public string FlightTime { get; set; }

        [JsonProperty("tourDescription")]
        public string TourDescription { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("cityTour")]
        public string CityTour { get; set; }

        [JsonProperty("licensePlate")]
        public string LicensePlate { get; set; }

        [JsonProperty("bookStatus")]
        public bool BookStatus { get; set; }

        [JsonProperty("numberPack")]
        public long NumberPack { get; set; }

        [JsonProperty("carType")]
        public string CarType { get; set; }

        [JsonProperty("contactNo")]
        public string ContactNo { get; set; }
    }
}
