using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ConsultaAcademica.Core;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Data.Interfaces;

namespace ConsultaAcademica.Data.Repositories
{
    public class LogImportacaoRepository : Database, ILogImportacaoRepository<LogImportacao>
    {
        private string TableName = "log_importacao";
        private string DateFormatting = "yyyy-MM-dd HH:mm:ss";

        public LogImportacaoRepository() : base()
        {

        }

        public LogImportacaoRepository(string connectionString) : base(connectionString)
        {

        }

        public LogImportacaoRepository(Sgbd sgbd) : base(sgbd)
        {

        }

        public LogImportacaoRepository(Sgbd sgbd, string connectionString) : base(sgbd, connectionString)
        {

        }

        public void Save(LogImportacao entity)
        {
            SetSgbd(Sgbd.MySql).
            ClearParameters().
            SetCommand(
                $"INSERT INTO {TableName} " +
                 "( " +
                 "     dt_importacao, " +
                 "     tp_importacao, " +
                 "     url, " +
                 "     dados_importados, " +
                 "     dados_retornados, " +
                 "     sucesso, " +
                 "     mensagem, " +
                 "     excecao, " +
                 "     sincronia, " +
                 "     suspenso " +
                 ") " +
                 "VALUES " +
                 "( " +
                 "     @dt_importacao, " +
                 "     @tp_importacao, " +
                 "     @url, " +
                 "     @dados_importados, " +
                 "     @dados_retornados, " +
                 "     @sucesso, " +
                 "     @mensagem, " +
                 "     @excecao, " +
                 "     @sincronia, " +
                 "     @suspenso " +
                 ")"
            )
            .AddParameterInput("@dt_importacao", entity.DataImportacao.ToString(DateFormatting))
            .AddParameterInput("@tp_importacao", entity.TipoImportacao.GetDbEnumValue())
            .AddParameterInput("@url", entity.Url)
            .AddParameterInput("@dados_importados", entity.DadosImportados)
            .AddParameterInput("@dados_retornados", entity.DadosRetornados)
            .AddParameterInput("@sucesso", entity.Sucesso ? 1 : 0)
            .AddParameterInput("@mensagem", entity.Mensagem)
            .AddParameterInput("@excecao", entity.Excecao)
            .AddParameterInput("@sincronia", entity.Sincronia ? 1 : 0)
            .AddParameterInput("@suspenso", entity.Suspenso ? 1 : 0)
            .Execute(true, true);
        }

        public void Save(IEnumerable<LogImportacao> data)
        {
            if(data?.Count() == 0)
            {
                return;
            }

            var builder = new StringBuilder();

            SetSgbd(Sgbd.MySql);
            SetCommandType(DatabaseCommandType.Text);
            ClearParameters();

            var columns =
                  @"(
                        dt_importacao,
                        tp_importacao,
                        url,
                        dados_importados,
                        dados_retornados,
                        sucesso,    
                        mensagem,
                        excecao,
                        sincronia,
                        suspenso
                    )";

            columns = columns.Replace(" ", "")
                             .Replace("\r", "")
                             .Replace("\n", "");

            builder.Append("INSERT INTO " + TableName + columns + " VALUES ");


            var result = Parallel.ForEach<LogImportacao>(data, (item) =>
            {
                lock (builder)
                {
                    var insert =
                        $@"(
                                @dt_importacao,
                                @tp_importacao,
                                @url,
                                @dados_importados,
                                @dados_retornados,
                                @sucesso,    
                                @mensagem,
                                @excecao,
                                @sincronia,
                                @suspenso
                           ),";

                    insert = " " + insert.Replace(" ", "")
                                         .Replace("\r", "")
                                         .Replace("\n", "");

                    insert = insert.Replace("@dt_importacao", item.DataImportacao.ToString(DateFormatting).Quote())
                                   .Replace("@tp_importacao", item.TipoImportacao.GetDbEnumValue().ToString().Quote())
                                   .Replace("@url", item.Url.Quote())
                                   .Replace("@dados_importados", item.DadosImportados.Quote())
                                   .Replace("@dados_retornados", item.DadosRetornados.Quote())
                                   .Replace("@sucesso", item.Sucesso ? "1" : "0")
                                   .Replace("@mensagem", item.Mensagem.Quote())
                                   .Replace("@excecao", item.Excecao.Quote())
                                   .Replace("@sincronia", item.Sincronia ? "1" : "0")
                                   .Replace("@suspenso", item.Suspenso ? "1" : "0");

                    builder.Append(insert);
                }
            });

            if (result.IsCompleted)
            {
                var sql = builder.ToString();
                sql = sql.Remove(sql.Length - 1, 1);
                sql += ";";

                SetCommand(sql).Execute(true);
            }
        }
    }
}
