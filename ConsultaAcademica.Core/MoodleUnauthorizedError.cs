using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public class MoodleUnauthorizedError
    {
        [JsonProperty("message")]
        public string Message { get { return "Serviço de importação Moodle recusou a requisição solicitada"; } }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("statuscode")]
        public HttpStatusCode StatusCode { get; set; }
    }
}
