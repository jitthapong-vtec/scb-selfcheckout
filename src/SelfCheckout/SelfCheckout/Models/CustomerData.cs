using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace SelfCheckout.Models
{
    public class CustomerData
    {
        [JsonProperty("Data")]
        public List<Data> Data { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }
        [JsonProperty("Tracking")]
        public Tracking Tracking { get; set; }
        [JsonProperty("Message")]
        public List<object> Message { get; set; }
    }

    public class Data
    {
        [JsonProperty("person")]
        public Person Person { get; set; }
        [JsonProperty("agentCode")]
        public string AgentCode { get; set; }
        [JsonProperty("subAgentCode")]
        public string SubAgentCode { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("isFound")]
        public bool IsFound { get; set; }
        [JsonProperty("pathURL")]
        public string PathURL { get; set; }
        [JsonProperty("stringToPrint")]
        public string StringToPrint { get; set; }
        [JsonProperty("pathURLMemberCard")]
        public string PathURLMemberCard { get; set; }
        [JsonProperty("listCoupon")]
        public List<ListCoupon> ListCoupon { get; set; }
    }

    public class ListContact
    {
        [JsonProperty("contactType")]
        public string ContactType { get; set; }
        [JsonProperty("contactValue")]
        public string ContactValue { get; set; }
    }

    public class ListCoupon
    {
        [JsonProperty("couponCode")]
        public string CouponCode { get; set; }
        [JsonProperty("couponDetail")]
        public string CouponDetail { get; set; }
        [JsonProperty("couponQRCode")]
        public string CouponQRCode { get; set; }
    }

    public class ListIdentity
    {
        [JsonProperty("IdentityType")]
        public string IdentityType { get; set; }
        [JsonProperty("IdentityValue")]
        public string IdentityValue { get; set; }
    }

    public class Person
    {
        [JsonProperty("runningNo")]
        public int RunningNo { get; set; }
        [JsonProperty("customerTypeCode")]
        public string CustomerTypeCode { get; set; }
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
        public DateTime FlightDate { get; set; }
        [JsonProperty("flightTime")]
        public string FlightTime { get; set; }
        [JsonProperty("flightRoute")]
        public string FlightRoute { get; set; }
        [JsonProperty("provinceCode")]
        public string ProvinceCode { get; set; }
        [JsonProperty("cityCode")]
        public string CityCode { get; set; }
        [JsonProperty("listContact")]
        public List<ListContact> ListContact { get; set; }
        [JsonProperty("listIdentity")]
        public List<ListIdentity> ListIdentity { get; set; }
        [JsonProperty("nationality")]
        public string Nationality { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [JsonProperty("order_status")]
        public object OrderStatus { get; set; }
        [JsonProperty("order_date")]
        public DateTime OrderDate { get; set; }
        [JsonProperty("fast_register")]
        public bool FastRegister { get; set; }
        [JsonProperty("isActivate")]
        public bool IsActivate { get; set; }
        [JsonProperty("PreRegister")]
        public bool PreRegister { get; set; }
        [JsonProperty("orderNo")]
        public int OrderNo { get; set; }
    }
}