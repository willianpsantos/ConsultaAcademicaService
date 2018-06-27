using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Configuration;

namespace ConsultaAcademica.Core
{
    public class EmailSender
    {
        private static string GetUrl()
        {
            return ConfigurationManager.AppSettings["sendmail.url"];
        }

        public static EmailResponse Send(Email email)
        {
            var url = GetUrl();
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            request.AddParameter("to", email.To);
            request.AddParameter("name", email.Username);
            request.AddParameter("subject", email.Subject);
            request.AddParameter("html", email.BodyIsHtmlFile);
            request.AddParameter("body", email.Body);

            var data = JsonConvert.SerializeObject(email.Data);
            request.AddParameter("data", data);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            var emailResponse = JsonConvert.DeserializeObject<EmailResponse>(content);
            return emailResponse;
        }

        public static Task<EmailResponse> SendAsync(Email email)
        {
            var func = new Func<EmailResponse>(() => Send(email));
            return Task.Factory.StartNew<EmailResponse>(func);
        }
    }
}
