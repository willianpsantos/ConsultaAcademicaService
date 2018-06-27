using ConsultaAcademica.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(Constantes.GET_BY_NAME_TAG, MoodleUrlDataConvertType.AsValue)]
    public class GetByNameRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
