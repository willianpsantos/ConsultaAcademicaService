using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ConsultaAcademica.Core;

namespace ConsultaAcademica.Core
{
    public abstract class AbstractService : IAbstractService
    {
        public string BaseUrl { get; set; }

        protected HttpClientFactory Factory { get; set; }

        public string Token { get; set; }

        public string ServiceName { get; set; }


        protected AbstractService()
        {
            Factory = new HttpClientFactory();
        }

        protected AbstractService(string baseUrl)
        {
            Factory = new HttpClientFactory();
            BaseUrl = baseUrl;
        }
        

        protected string FormatParameters(IDictionary<string, string> parameters)
        {
            string formatedParameters = "";
            
            foreach(KeyValuePair<string, string> item in parameters)
            {
                var keys = parameters.Keys.ToList();

                if(keys.IndexOf(item.Key) == 0)
                {
                    formatedParameters += $"?{item.Key}={WebUtility.UrlEncode(item.Value)}";
                }
                else
                {
                    formatedParameters += $"&{item.Key}={WebUtility.UrlEncode(item.Value)}";
                }
            }

            return formatedParameters;
        }

        protected async Task<string> InternalGet(string url, IDictionary<string, string> parameters)
        {            
            HttpClient client = Factory.CreateHttpClient(this.BaseUrl);
            string completeurl = this.BaseUrl + url;

            if (parameters != null && parameters.Count > 0)
            {
                completeurl = this.BaseUrl + url + this.FormatParameters(parameters);
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, completeurl);

            if (!string.IsNullOrEmpty(Token) && !string.IsNullOrWhiteSpace(Token))
            {
                request.Headers.Add("Authorization", Token);
            }

            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            HttpResponseMessage ensureResponse = response.EnsureSuccessStatusCode();
            string content = await ensureResponse.Content.ReadAsStringAsync();
            return content;
        }

        protected async Task<string> InternalPost(string url, IDictionary<string, string> parameters)
        {
            HttpClient client = Factory.CreateHttpClient(BaseUrl);
            string completeurl = this.BaseUrl + url;

            if (!string.IsNullOrEmpty(Token) && !string.IsNullOrWhiteSpace(Token))
            {
                client.DefaultRequestHeaders.Add("Authorization", Token);
            }

            FormUrlEncodedContent httpContent = new FormUrlEncodedContent(parameters);           
            HttpResponseMessage response = await client.PostAsync(completeurl, httpContent);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "";
            }

            HttpResponseMessage ensureResponse = response.EnsureSuccessStatusCode();
            string content = await ensureResponse.Content.ReadAsStringAsync();
            return content;
        }

        protected async Task<string> InternalDelete(string url, IDictionary<string, string> parameters)
        {
            HttpClient client = Factory.CreateHttpClient(BaseUrl);
            string completeurl = this.BaseUrl + url;

            if (!string.IsNullOrEmpty(Token) && !string.IsNullOrWhiteSpace(Token))
            {
                client.DefaultRequestHeaders.Add("Authorization", Token);
            }

            if (parameters != null && parameters.Count > 0)
            {
                completeurl = this.BaseUrl + url + this.FormatParameters(parameters);
            }

            HttpResponseMessage response = await client.DeleteAsync(completeurl);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "";
            }

            HttpResponseMessage ensureResponse = response.EnsureSuccessStatusCode();
            var content = await ensureResponse.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Get(string url)
        {
            return await InternalGet(url, null);
        }

        public async Task<string> Get(string url, IDictionary<string, string> parameters)
        {
            return await InternalGet(url, parameters);
        }

        public async Task<string> Post(string url)
        {
            return await InternalPost(url, null);
        }

        public async Task<string> Post(string url, IDictionary<string, string> parameters)
        {
            return await InternalPost(url, parameters);
        }

        public async Task<string> Delete(string url)
        {
            return await InternalDelete(url, null);
        }

        public async Task<string> Delete(string url, IDictionary<string, string> parameters)
        {
            return await InternalDelete(url, parameters);
        }
    }
}
