using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Model;
using Newtonsoft.Json;

namespace ConsultaAcademica.Proxy
{
    public class AlunoDisciplinaNotImportedReason<TData> : NotImportedReason<TData>
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
                DadosRetornados = "",
                Url = Url,
                Sucesso = false,
                Mensagem = Reason,
                Excecao = Exception != null ? JsonConvert.SerializeObject(Exception) : ""
            };

            return log;
        }
    }
}
