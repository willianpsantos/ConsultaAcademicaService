using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Data.Interfaces;
using ConsultaAcademica.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Proxy
{
    public class AgendamentoExecucaoServicoProxy
    {
        private IAgendamentoExecucaoServicoRepository Repository;

        public AgendamentoExecucaoServicoProxy(IAgendamentoExecucaoServicoRepository repository)
        {
            Repository = repository;
        }

        public IEnumerable<AgendamentoExecucaoServico> GetData()
        {
            var data = Repository.GetAgendamentos();
            return data;
        }
    }
}
