using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace ConsultaAcademica.Core
{
    public class MoodleServiceClient<TRequest, TResponse> : AbstractMoodleServiceClient, IDisposable where TResponse : class where TRequest  : class
    {
        public MoodleServiceClient() : base()
        {
           
        }

        public MoodleServiceClient(string function, string tag) : base(function, tag)
        {

        }

        public MoodleServiceClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }

        public new MoodleServiceClient<TRequest, TResponse> AddToken(string token)
        {
            Token = token;
            return this;
        }

        public new MoodleServiceClient<TRequest, TResponse> AddServiceUrl(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
            return this;
        }

        public new MoodleServiceClient<TRequest, TResponse> AddBaseUrl(string baseUrl)
        {
            BaseUrl = baseUrl;
            return this;
        }

        public new MoodleServiceClient<TRequest, TResponse> AddHttpClient(HttpClient client)
        {
            Client = client;
            return this;
        }

        public async Task<TResponse> Post(TRequest data)
        {
            string json = string.Empty;
            HttpClient client = CreateHttpClient();

            try
            {
                string queryStrings = ConvertData<TRequest>(data);
                LastUrl = GetBaseUrl() + (!string.IsNullOrEmpty(queryStrings) ? "&" + queryStrings : "");
                var response = await client.PostAsync(LastUrl, null);
                json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new MoodleUnauthorizedRequestException(LastUrl, response.StatusCode);
                }

                if (json.IsNullResponse())
                {
                    return default(TResponse);
                }

                if (!string.IsNullOrEmpty(json) && !string.IsNullOrWhiteSpace(json))
                {
                    if (json.IsXml() || !json.IsJson() || json.IsMoodleError())
                    {
                        throw new MoodleResponseException(json);
                    }
                }

                var responseType = typeof(TResponse);

                if (responseType == typeof(EmptyResponse))
                {
                    return (TResponse)responseType.Assembly.CreateInstance(responseType.FullName);
                }

                var interfaces = responseType.GetInterfaces();
                var collectionInterfaceType = typeof(ICollection<TResponse>);
                var hasCollectionInterface = interfaces.Where(x => x.Equals(collectionInterfaceType)).Count() > 0;

                if (responseType.IsArray || hasCollectionInterface)
                {
                    var multipleResult = JsonConvert.DeserializeObject<TResponse>(json);
                    return multipleResult;
                }

                TResponse[] singleResult = JsonConvert.DeserializeObject<TResponse[]>(json);
                return singleResult?.Length > 0 ? singleResult[0] : default(TResponse);
            }
            catch (WebException)
            {
                return await Post(data);
            }
            catch (TaskCanceledException)
            {
                return await Post(data);
            }
            catch (MoodleResponseException mex)
            {
                if(mex.MoodleErrorData == null)
                {
                    throw;
                }

                // if the exception returned by moodle is some exception related with database, then the service try to resend data.
                if(mex.RawMoodleError.ToLower().Contains("dmlreadexception") ||
                    mex.RawMoodleError.ToLower().Contains("dml_read_exception") ||
                     mex.RawMoodleError.ToLower().Contains("dmlwriteexception") ||
                      mex.RawMoodleError.ToLower().Contains("dml_write_exception") ||
                       mex.RawMoodleError.ToLower().Contains("dmlconnectionexception") ||
                        mex.RawMoodleError.ToLower().Contains("dml_connection_exception") ||
                         mex.RawMoodleError.ToLower().Contains("Database connection failed"))
                {
                    return await Post(data);
                }

                throw;
            }
            catch (Exception ex)
            {                
                throw;
            }            
        }

        public void Dispose()
        {
            Token = "";
            ServiceUrl = "";
            BaseUrl = "";
            Function = "";
            Tag = "";
            LastUrl = "";

            Client?.CancelPendingRequests();
            Client?.Dispose();
        }
    }
}
