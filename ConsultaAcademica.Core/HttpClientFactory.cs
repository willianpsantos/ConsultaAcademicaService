using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class HttpClientFactory
    {
        public HttpClient CreateHttpClient(string baseUrl)
        {
            var client = new HttpClient();            
            client.BaseAddress = new Uri(baseUrl);
            return client;
        }

        public HttpClient CreateMoodleHttpClient()
        {
            HttpClient client = new HttpClient()
            {
                MaxResponseContentBufferSize = 2147483647,
                Timeout = TimeSpan.FromMinutes(60)
            };

            ServicePointManager.DefaultConnectionLimit = 100000;
            client.Timeout = TimeSpan.FromMinutes(30);
            client.DefaultRequestHeaders.ConnectionClose = true;            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

            return client;
        }
    }
}
