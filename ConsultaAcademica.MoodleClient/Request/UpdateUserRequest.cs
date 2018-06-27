using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultaAcademica.Core;
using Newtonsoft.Json;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(Constantes.USERS_TAG, MoodleUrlDataConvertType.AsArray)]
    public class UpdateUserRequest : UserRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("suspended")]
        public byte Suspended { get; set; }
    }
}
