using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Professor
    {
        public Professor()
        {
            Disciplinas = new List<Disciplina>();
        }

        [JsonProperty("IdProfessor")]
        public long IdProfessor { get; set; }

        [JsonProperty("ProfessorNome")]
        public string ProfessorNome { get; set; }

        [JsonProperty("ProfessorCpf")]
        public string ProfessorCpf { get; set; }

        [JsonProperty("ProfessorEmail")]
        public string ProfessorEmail { get; set; }

        [JsonProperty("ProfessorMatricula")]
        public string ProfessorMatricula { get; set; }

        [JsonProperty("ProfessorPrincipal")]
        public bool ProfessorPrincipal { get; set; }

        [JsonProperty("AtivoProfessor")]
        public bool AtivoProfessor { get; set; }

        public IEnumerable<Disciplina> Disciplinas { get; set; }
    }
}
