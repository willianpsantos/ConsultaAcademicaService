using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ConsultaAcademica.Model
{
    public class Aluno : AlunoCodigo
    {
        public Aluno()
        {
            Disciplinas = new List<Disciplina>();
        }

        [JsonProperty("AlunoNome")]
        public string AlunoNome { get; set; }

        [JsonProperty("AlunoNomeSocial")]
        public string AlunoNomeSocial { get; set; }

        [JsonProperty("AlunoCpf")]
        public string AlunoCpf { get; set; }

        [JsonProperty("AlunoEmail")]
        public string AlunoEmail { get; set; }

        [JsonProperty("AlunoMatricula")]
        public string AlunoMatricula { get; set; }

        [JsonProperty("AlunoSexo")]
        public string AlunoSexo { get; set; }

        [JsonProperty("AlunoNascimento")]
        public string AlunoNascimento { get; set; }

        [JsonProperty("AlunoTelefone")]
        public string AlunoTelefone { get; set; }

        [JsonProperty("IdCampus")]
        public long IdCampus { get; set; }

        [JsonProperty("CampusDescricao")]
        public string CampusDescricao { get; set; }

        [JsonProperty("IdCurso")]
        public long IdCurso { get; set; }

        [JsonProperty("CursoDescricao")]
        public string CursoDescricao { get; set; }

        [JsonProperty("CursoSigla")]
        public string CursoSigla { get; set; }

        [JsonProperty("IdTurma")]
        public long IdTurma { get; set; }

        [JsonProperty("TurmaSigla")]
        public string TurmaSigla { get; set; }

        [JsonProperty("IdTurno")]
        public string IdTurno { get; set; }

        [JsonProperty("TurnoDescricao")]
        public string TurnoDescricao { get; set; }

        [JsonProperty("IdModalidade")]
        public long IdModalidade { get; set; }

        [JsonProperty("ModalidadeDescricao")]
        public string ModalidadeDescricao { get; set; }

        [JsonProperty("PeriodoLetivoDescricao")]
        public string PeriodoLetivoDescricao { get; set; }

        [JsonProperty("SituacaoAcademicaNome")]
        public string SituacaoAcademicaNome { get; set; }

        [JsonProperty("AtivoAluno")]
        public bool AtivoAluno { get; set; }

        [JsonIgnore]
        public IEnumerable<Disciplina> Disciplinas { get; set; }
    }
}
