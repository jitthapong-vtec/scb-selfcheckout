using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfCheckout.Models
{
    public class ArticleImage
    {
        [JsonProperty("article_code")]
        public string Code { get; set; }
        [JsonProperty("article_img")]
        public string ImageUrl { get; set; }
    }
}
