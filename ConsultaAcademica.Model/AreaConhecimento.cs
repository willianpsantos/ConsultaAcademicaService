using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class AreaConhecimento
    { 
        [JsonProperty("IdGpa")]
        public long IdGpa { get; set; }

        [JsonProperty("GpaDescricao")]
        public string GpaDescricao { get; set; }

        [JsonProperty("GpaSigla")]
        public string GpaSigla { get; set; }

        [JsonProperty("IdDiretor")]
        public long IdDiretor { get; set; }
    }
}
