using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class AlunoCodigo
    {
        [JsonProperty("IdAluno")]
        public long IdAluno { get; set; }
    }
}
