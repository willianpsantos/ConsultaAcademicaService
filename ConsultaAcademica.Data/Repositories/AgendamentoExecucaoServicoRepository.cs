using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Data.Interfaces;

namespace ConsultaAcademica.Data.Repositories
{
    public class AgendamentoExecucaoServicoRepository : Database, IAgendamentoExecucaoServicoRepository
    {
        public IEnumerable<AgendamentoExecucaoServico> GetAgendamentos()
        {
            SetSgbd(Sgbd.MySql);
            ClearParameters();

            SetCommand("SELECT * FROM agendamento_execucao_servico");
            return ToArray<AgendamentoExecucaoServico>();
        }
    }
}
