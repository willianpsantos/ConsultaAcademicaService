using ConsultaAcademica.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Data.Interfaces
{
    public interface IAgendamentoExecucaoServicoRepository
    {
        IEnumerable<AgendamentoExecucaoServico> GetAgendamentos();
    }
}
