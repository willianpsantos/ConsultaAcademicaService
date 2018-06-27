using ConsultaAcademica.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(Constantes.GET_BY_ID_TAG, MoodleUrlDataConvertType.AsValue)]
    public class GetByIdRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
