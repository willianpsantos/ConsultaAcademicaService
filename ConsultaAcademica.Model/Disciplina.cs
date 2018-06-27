using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Disciplina
    {
        [JsonProperty("IdDisciplina")]
        public long IdDisciplina { get; set; }

        [JsonProperty("DisciplinaNome")]
        public string DisciplinaNome { get; set; }

        [JsonProperty("DisciplinaSigla")]
        public string DisciplinaSigla { get; set; }

        [JsonProperty("IdCurso")]
        public long IdCurso { get; set; }

        [JsonProperty("CursoDescricao")]
        public string CursoDescricao { get; set; }

        [JsonProperty("CursoSigla")]
        public string CursoSigla { get; set; }

        [JsonProperty("TurmaSigla")]
        public string TurmaSigla { get; set; }

        [JsonProperty("TurnoDescricao")]
        public string TurnoDescricao { get; set; }

        [JsonProperty("IdModalidade")]
        public long IdModalidade { get; set; }

        [JsonProperty("ModalidadeDescricao")]
        public string ModalidadeDescricao { get; set; }

        [JsonProperty("VinculoProfessor")]
        public VinculoProfessor[] VinculoProfessor { get; set; }

        [JsonProperty("ShortName")]
        public string ShortName { get; set; }
    }
}
