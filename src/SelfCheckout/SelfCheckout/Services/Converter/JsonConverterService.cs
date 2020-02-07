using Newtonsoft.Json;
using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Converter
{
    public class JsonConverterService : IConverterService
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonConverterService()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new NullToEmptyStringResolver()
            };
        }

        public async Task<TResult> Convert<TResult>(string data)
        {
            return await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(data, _serializerSettings));
        }
    }
}
