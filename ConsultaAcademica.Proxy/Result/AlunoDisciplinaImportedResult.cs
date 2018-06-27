using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Model;
using Newtonsoft.Json;

namespace ConsultaAcademica.Proxy
{
    public class AlunoDisciplinaImportedResult<TData, TResponse> : ImportedResult<TData, TResponse>
    {
        public Aluno Aluno { get; set; }

        public Disciplina Disciplina { get; set; }

        public LogImportacao Parse()
        {
            var result = new
            {
                aluno = Aluno,
                disciplina = Disciplina
            };

            var log = new LogImportacao()
            {
                DataImportacao = this.Date,
                TipoImportacao = TipoImportacao.DisciplinaAluno,
                DadosImportados = JsonConvert.SerializeObject(result),
                DadosRetornados = JsonConvert.SerializeObject(Result),
                Url = Url,
                Sucesso = true,
                Mensagem = $"Criação e vinculo da disciplina [{result.disciplina.DisciplinaNome}] com o aluno [{result.aluno.AlunoCpf}][{result.aluno.AlunoNome}]"
            };

            return log;
        }
    }
}
