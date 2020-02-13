using Newtonsoft.Json;
using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Serializer
{
    public class JsonSerializeService : ISerializeService
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonSerializeService()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new NullToEmptyStringResolver()
            };
        }

        public async Task<TResult> Serialize<TResult>(string data)
        {
            return await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(data, _serializerSettings));
        }
    }
}
