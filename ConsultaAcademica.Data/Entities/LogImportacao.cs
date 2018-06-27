using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using Newtonsoft.Json;

namespace ConsultaAcademica.Data.Entities
{
    public enum TipoImportacao
    {
        [EnumValueAttribute("curso")]
        Curso,

        [EnumValueAttribute("disciplina")]
        Disciplina,

        [EnumValueAttribute("disciplina_aluno")]
        DisciplinaAluno,

        [EnumValueAttribute("disciplina_professor")]
        DisciplinaProfessor,

        [EnumValueAttribute("aluno")]
        Aluno,

        [EnumValueAttribute("professor")]
        Professor
    }

    public class LogImportacao
    {
        [Column("dt_importacao")]
        public DateTime DataImportacao { get; set; }

        [Column("tp_importacao")]
        public TipoImportacao TipoImportacao { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("dados_importados")]
        public string DadosImportados { get; set; }

        [Column("dados_retornados")]
        public string DadosRetornados { get; set; }

        [Column("sucesso")]
        public bool Sucesso { get; set; }

        [Column("mensagem")]
        public string Mensagem { get; set; }

        [Column("excecao")]
        public string Excecao { get; set; }

        [Column("sincronia")]
        public bool Sincronia { get; set; }

        [Column("suspenso")]
        public bool Suspenso { get; set; }

        public static LogImportacao Parse<TData, TResponse>(ImportedResult<TData, TResponse> item, TipoImportacao tipoImportacao)
        {
            var log = new LogImportacao()
            {
                DataImportacao = item.Date,
                TipoImportacao = tipoImportacao,
                DadosImportados = JsonConvert.SerializeObject(item.Data),
                DadosRetornados = JsonConvert.SerializeObject(item.Result),
                Url = item.Url,
                Sucesso = true,
                Suspenso = !item.Active
            };

            return log;
        }

        public static LogImportacao Parse<TData>(NotImportedReason<TData> item, TipoImportacao tipoImportacao)
        {
            var log = new LogImportacao()
            {
                DataImportacao = item.Date,
                TipoImportacao = tipoImportacao,
                DadosImportados = JsonConvert.SerializeObject(item.Data),
                DadosRetornados = "",
                Url = item.Url,
                Sucesso = false,                
                Mensagem = item.Reason,
                Excecao = item.Exception != null ? JsonConvert.SerializeObject(item.Exception) : ""
            };

            return log;
        }
    }
}
