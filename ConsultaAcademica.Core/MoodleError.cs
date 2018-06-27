using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public class MoodleError
    {
        [JsonProperty("errorcode")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("debuginfo")]
        public string DebugInfo { get; set; }

        [JsonProperty("exception")]
        public string Exception { get; set; }
    }
}
