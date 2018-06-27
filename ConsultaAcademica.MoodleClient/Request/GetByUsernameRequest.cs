using ConsultaAcademica.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(Constantes.GET_BY_USERNAME_TAG, MoodleUrlDataConvertType.AsValue)]
    public class GetByUsernameRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
