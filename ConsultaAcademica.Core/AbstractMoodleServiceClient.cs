using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsultaAcademica.Core
{
    public class EmptyResponse
    {
        private string Message { get { return "Request with no response"; } }
    }

    public class EmptyRequest
    {
        private string Message { get { return "Request with no parameters"; } }
    }

    public abstract class AbstractMoodleServiceClient
    {
        public string Token { get; set; }

        public string ServiceUrl { get; set; }

        public string BaseUrl { get; set; }

        public string Function { get; set; }

        public string Tag { get; set; }

        public string LastUrl { get; protected set; }

        public string Operation { get; set; }

        protected HttpClient Client { get; set; }


        public AbstractMoodleServiceClient()
        {

        }

        public AbstractMoodleServiceClient(string function, string tag)
        {
            Function = function;
            Tag = tag;
        }

        public AbstractMoodleServiceClient(string function, string tag, string operation)
        {
            Function = function;
            Tag = tag;
            Operation = operation;
        }


        public virtual AbstractMoodleServiceClient AddToken(string token)
        {
            Token = token;
            return this;
        }

        public virtual AbstractMoodleServiceClient AddServiceUrl(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
            return this;
        }

        public virtual AbstractMoodleServiceClient AddBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl;
            return this;
        }

        public virtual AbstractMoodleServiceClient AddHttpClient(HttpClient client)
        {
            Client = client;
            return this;
        }


        public HttpClient GetUnderlyingHttpClient()
        {
            return Client;
        }

        protected string GetBaseUrl()
        {
            return $"{BaseUrl}{ServiceUrl}?wstoken={Token}&wsfunction={Function}&moodlewsrestformat=json";
        }

        protected HttpClient CreateHttpClient()
        {
            if (Client != null)
            {
                return Client;
            }

            var factory = new HttpClientFactory();
            Client = factory.CreateMoodleHttpClient();
            return Client;
        }

        public string ConvertData<TData>(TData data)
        {
            var type = typeof(TData);
            var properties = type.GetProperties();
            var urlParams = new List<string>(properties.Length);
            var param = string.Empty;
            var moodleTag = Tag;
            var moodleParamConvertType = MoodleUrlDataConvertType.AsArray;

            var attributes = type.GetCustomAttributes(typeof(MoodleUrlDataRequestAttribute), false);

            if (attributes?.Length > 0)
            {
                var moodleUrlAndTag = (MoodleUrlDataRequestAttribute)attributes[0];
                moodleTag = moodleUrlAndTag.Tag;
                moodleParamConvertType = moodleUrlAndTag.ConvertType;
            }

            for (var p = 0; p < properties.Length; p++)
            {
                var property = properties[p];
                var name = property.Name.ToLower();
                var value = property.GetValue(data);

                if (property.PropertyType.Equals(typeof(NameValueCollection)))
                {
                    var preferences = (NameValueCollection)value;
                    param = string.Empty;

                    for (var e = 0; e < preferences.Count; e++)
                    {
                        var key = preferences.GetKey(e);
                        var pref = preferences.Get(key);
                        param += $"{moodleTag}[0][preferences][{e}][type]={key}&{moodleTag}[0][preferences][{e}][value]={pref}";
                    }
                }
                else
                {
                    switch (moodleParamConvertType)
                    {
                        case MoodleUrlDataConvertType.AsArray:
                        default:
                            param = $"{moodleTag}[0][{name}]={value}";
                            break;
                        case MoodleUrlDataConvertType.AsValue:
                            param = $"{name}={value}";
                            break;
                    }
                }

                urlParams.Add(param);
            }

            return urlParams?.Count > 0 ? string.Join("&", urlParams.ToArray()) : "";
        }

        
    }
}
