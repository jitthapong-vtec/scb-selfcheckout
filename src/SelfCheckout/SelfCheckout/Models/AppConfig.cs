using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class AppConfig
    {
        string _urlSaleEngineApi;
        string _urlRegisterApi;

        [JsonProperty("show_article_img")]
        public bool ShowArticleImage { get; set; }
        [JsonProperty("url_register_api")]
        public string UrlRegisterApi
        {
            get
            {
                if (!_urlRegisterApi.EndsWith("/"))
                    _urlRegisterApi += "/";
                return _urlRegisterApi;
            }
            set
            {
                _urlRegisterApi = value;
            }
        }
        [JsonProperty("payment_timeout")]
        public int PaymentTimeout { get; set; }
        [JsonProperty("url_saleengine_api")]
        public string UrlSaleEngineApi
        {
            get
            {
                if (!_urlSaleEngineApi.EndsWith("/"))
                    _urlSaleEngineApi += "/";
                return _urlSaleEngineApi;
            }
            set
            {
                _urlSaleEngineApi = value;
            }
        }
        [JsonProperty("module")]
        public string Module { get; set; }
        [JsonProperty("branch_no")]
        public string BranchNo { get; set; }
        [JsonProperty("sub_branch")]
        public string SubBranch { get; set; }
        [JsonProperty("url_member_api")]
        public string UrlMemberApi { get; set; }
        [JsonProperty("url_member_web")]
        public string UrlMemberWeb { get; set; }
    }
}
