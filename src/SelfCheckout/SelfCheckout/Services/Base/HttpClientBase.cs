using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SelfCheckout.Models;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Base
{
    public abstract class HttpClientBase
    {
        ISerializeService _converterService;

        enum RequestTypes
        {
            Get,
            Post,
            Put,
            Delete
        }

        HttpClient _httpClient;

        public HttpClientBase(ISerializeService converterService)
        {
            _converterService = converterService;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        public TimeSpan RequestTimeout
        {
            set => _httpClient.Timeout = value;
        }

        protected void SetRequestHeader(string key, string val)
        {
            _httpClient.DefaultRequestHeaders.Add(key, val);
        }

        public async Task<TResult> GetAsync<TResult>(string uri)
        {
            return await RequestAsync<object, TResult>(uri, RequestTypes.Get, null);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data)
        {
            return await RequestAsync<TRequest, TResult>(uri, RequestTypes.Post, data);
        }

        public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data)
        {
            return await RequestAsync<TRequest, TResult>(uri, RequestTypes.Put, data);
        }

        public async Task<TResult> DeleteAsync<TRequest, TResult>(string uri, TRequest data)
        {
            return await RequestAsync<TRequest, TResult>(uri, RequestTypes.Delete, data);
        }

        private async Task<TResult> RequestAsync<TRequest, TResult>(string uri, RequestTypes requestType, TRequest data)
        {
            StringContent content = null;
            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            HttpResponseMessage response = null;
            try
            {
                if (requestType == RequestTypes.Get)
                    response = await _httpClient.GetAsync(uri);
                else if (requestType == RequestTypes.Post)
                    response = await _httpClient.PostAsync(uri, content);
                else if (requestType == RequestTypes.Put)
                    response = await _httpClient.PutAsync(uri, content);
                else if (requestType == RequestTypes.Delete)
                    response = await _httpClient.DeleteAsync(uri);
            }
            catch (TaskCanceledException)
            {
                throw new HttpRequestExceptionEx(HttpStatusCode.RequestTimeout, "Connection timeout");
            }

            await HandleResponse(response);
            var result = await response.Content.ReadAsStringAsync();
            return await _converterService.Serialize<TResult>(result);
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }
    }
}
