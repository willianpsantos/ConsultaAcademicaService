using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ConsultaAcademica.Core;

namespace ConsultaAcademica.Data.Entities
{
    public class AgendamentoExecucaoServico
    {
        [Column("dt_execucao")]
        public string DataExecucaoString { get; set; }

        public bool ExecutaTodosOsDias { get; private set; }

        public DateTime? DataExecucao
        {
            get
            {
                ExecutaTodosOsDias = false;

                if (string.IsNullOrEmpty(DataExecucaoString) || string.IsNullOrWhiteSpace(DataExecucaoString) || DataExecucaoString == "*")
                {
                    ExecutaTodosOsDias = true;
                    return null;
                }

                DateTime date;
                bool success = DateTime.TryParse(DataExecucaoString, out date);

                if(!success)
                {
                    ExecutaTodosOsDias = true;
                    return null;
                }

                return date;
            }
        }

        [Column("hora_execucao")]
        public string HoraExecucaoString { get; set; }

        public DateTime? DataHoraExecucacao
        {
            get
            {
                if (string.IsNullOrEmpty(HoraExecucaoString) || string.IsNullOrWhiteSpace(HoraExecucaoString))
                    return null;

                if (!Regex.IsMatch(HoraExecucaoString, @"^[\d]{2}\:{1}[\d]{2}$"))
                    return null;

                var date = ExecutaTodosOsDias ? DateTime.Now.ToShortDateString() : DataExecucao.GetValueOrDefault().ToShortDateString();
                date += $" {HoraExecucaoString}";

                DateTime hour;
                bool sucesso = DateTime.TryParse(date, out hour);

                if (!sucesso)
                    return null;

                return hour;
            }
        }

        public bool PrecisaSerExecutado
        {
            get
            {                
                if (!DataHoraExecucacao.HasValue)
                    return false;

                var value = DataHoraExecucacao.GetValueOrDefault();
                return (value.DateEquals(DateTime.Now));
            }
        }

        [Column("executa_sincronia")]
        public bool ExecutaSincronia { get; set; }

        [Column("executa_completo")]
        public bool ExecutaCompleto { get; set; }
    }
}
