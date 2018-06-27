using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class VinculoProfessor
    {
        [JsonProperty("IdProfessor")]
        public long IdProfessor { get; set; }

        [JsonProperty("ProfessorNome")]
        public string ProfessorNome { get; set; }

        [JsonProperty("ProfessorPrincipal")]
        public bool ProfessorPrincipal { get; set; }
    }
}
