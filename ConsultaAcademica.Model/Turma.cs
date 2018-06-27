using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Turma
    {
        [JsonProperty("IdTurma")]
        public long IdTurma { get; set; }

        [JsonProperty("TurmaSigla")]
        public string TurmaSigla { get; set; }

        [JsonProperty("IdTurno")]
        public long IdTurno { get; set; }

        [JsonProperty("TurnoDescricao")]
        public string TurnoDescricao { get; set; }

        [JsonProperty("IdCurso")]
        public long IdCurso { get; set; }

        [JsonProperty("CursoDescricao")]
        public string CursoDescricao { get; set; }

        [JsonProperty("AtivoTurma")]
        public bool AtivoTurma { get; set; }
    }
}
