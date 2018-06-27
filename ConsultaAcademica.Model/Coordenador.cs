using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Coordenador
    {
        [JsonProperty("IdCoordenador")]
        public long IdCoordenador { get; set; }

        [JsonProperty("CoordenadorNome")]
        public string CoordenadorNome { get; set; }

        [JsonProperty("CoordenadorCpf")]
        public string CoordenadorCpf { get; set; }

        [JsonProperty("CoordenadorEmail")]
        public string CoordenadorEmail { get; set; }

        [JsonProperty("CoordenadorMatricula")]
        public string CoordenadorMatricula { get; set; }

        [JsonProperty("AtivoCoordenador")]
        public bool AtivoCoordenador { get; set; }
    }
}
