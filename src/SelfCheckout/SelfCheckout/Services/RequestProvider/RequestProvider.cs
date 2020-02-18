using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SelfCheckout.Models;
using SelfCheckout.Services.Serializer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SelfCheckout.Services.RequestProvider
{
    public class RequestProvider : IRequestProvider
    {
        ISerializeService _converterService;

        enum RequestTypes
        {
            Get,
            Post,
            Put,
            Delete
        }

        public RequestProvider(ISerializeService converterService)
        {
            _converterService = converterService;
        }

        public async Task<TResult> GetAsync<TResult>(string uri, string accessToken = "")
        {
            return await RequestAsync<object, TResult>(uri, RequestTypes.Get, null, accessToken);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string accessToken = "")
        {
            return await RequestAsync<TRequest, TResult>(uri, RequestTypes.Post, data, accessToken);
        }

        private async Task<TResult> RequestAsync<TRequest, TResult>(string uri, RequestTypes requestType, TRequest data, string accessToken)
        {
            HttpClient httpClient = CreateHttpClient();

            if (!string.IsNullOrEmpty(accessToken))
            {
                SetAccessToken(httpClient, accessToken);
            }

            var content = new StringContent(string.Empty);
            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                content = new StringContent(json);
            }

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = null;
            try
            {
                if (requestType == RequestTypes.Get)
                    response = await httpClient.GetAsync(uri);
                else if (requestType == RequestTypes.Post)
                    response = await httpClient.PostAsync(uri, content);
                else if (requestType == RequestTypes.Put)
                    response = await httpClient.PutAsync(uri, content);
                else if (requestType == RequestTypes.Delete)
                    response = await httpClient.DeleteAsync(uri);
            }
            catch (TaskCanceledException)
            {
                throw new HttpRequestExceptionEx(HttpStatusCode.RequestTimeout, "Connection timeout");
            }

            await HandleResponse(response);
            var result = await response.Content.ReadAsStringAsync();
            return await _converterService.Serialize<TResult>(result);
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            return httpClient;
        }

        private void SetAccessToken(HttpClient httpClient, string accessToken)
        {
            if (httpClient == null)
                return;

            if (string.IsNullOrEmpty(accessToken))
                return;
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            httpClient.DefaultRequestHeaders.Add("CallerID", "SCBCHECKOUT");
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }

        public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data, string accessToken = "")
        {
            HttpClient httpClient = CreateHttpClient();

            if (!string.IsNullOrEmpty(accessToken))
            {
                SetAccessToken(httpClient, accessToken);
            }

            var content = new StringContent(string.Empty);
            if (data != null)
                content = new StringContent(JsonConvert.SerializeObject(data));

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = null;
            try
            {
                response = await httpClient.PutAsync(uri, content);
            }
            catch (TaskCanceledException)
            {
                throw new HttpRequestExceptionEx(HttpStatusCode.RequestTimeout, "Connection timeout");
            }

            await HandleResponse(response);
            var result = await response.Content.ReadAsStringAsync();
            return await _converterService.Serialize<TResult>(result);
        }
    }
}
