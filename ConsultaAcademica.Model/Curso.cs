using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Curso
    {
        [JsonProperty("IdCurso")]
        public long IdCurso { get; set; }

        [JsonProperty("CursoDescricao")]
        public string CursoDescricao { get; set; }

        [JsonProperty("CursoSigla")]
        public string CursoSigla { get; set; }

        [JsonProperty("CodigoInep")]
        public string CodigoInep { get; set; }

        [JsonProperty("IdGpa")]
        public long IdGpa { get; set; }

        [JsonProperty("GpaDescricao")]
        public string GpaDescricao { get; set; }

        [JsonProperty("IdModalidade")]
        public long IdModalidade { get; set; }

        [JsonProperty("ModalidadeDescricao")]
        public string ModalidadeDescricao { get; set; }        
    }
}
