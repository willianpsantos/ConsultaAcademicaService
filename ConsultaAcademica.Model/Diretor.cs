using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Diretor
    {
        [JsonProperty("IdDiretor")]
        public long IdDiretor { get; set; }

        [JsonProperty("DiretorNome")]
        public string DiretorNome { get; set; }

        [JsonProperty("DiretorCpf")]
        public string DiretorCpf { get; set; }

        [JsonProperty("DiretorEmail")]
        public string DiretorEmail { get; set; }

        [JsonProperty("DiretorMatricula")]
        public string DiretorMatricula { get; set; }

        [JsonProperty("AtivoDiretor")]
        public bool AtivoDiretor { get; set; }
    }
}
