using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConsultaAcademica.Proxy
{
    public abstract class BaseParallelConsultaAcademicaProxy<TData, TResponse> : AbstractProxy, IAbstractParallelProxy<TData, TResponse>
        where TData : class, new() 
        where TResponse : class, new()
    {
        protected IConsultaAcademicaService ConsultaAcademicaService;
        protected IEnumerable<Modalidade> Modalidades;
        protected string SemestreAtual;
        protected ParallelSendResult<TData, TResponse> Result;

        public bool UseParallelism { get; set; }


        public BaseParallelConsultaAcademicaProxy(IAbstractService abstractService) : base(abstractService)
        {
            ConsultaAcademicaService = (IConsultaAcademicaService)Service;
            UseParallelism = false;
            CanLog = false;
        }


        protected abstract void ProcessItem(TData data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient);

        protected abstract void ProcessWithParallelism(IEnumerable<TData> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient);

        protected abstract void ProcessWithRegularForeach(IEnumerable<TData> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient);
        

        public BaseParallelConsultaAcademicaProxy<TData, TResponse> AddModalidades(IEnumerable<Modalidade> modalidades)
        {
            Modalidades = modalidades;
            return this;
        }

        public BaseParallelConsultaAcademicaProxy<TData, TResponse> AddSemestreAtual(string semestre)
        {
            SemestreAtual = semestre;
            return this;
        }

        public virtual IEnumerable<TData> GetData(string filter, bool active = true)
        {
            throw new NotImplementedException();
        }

        public virtual TResponse SendItem(AbstractMoodleServiceClient client, TData item)
        {
            throw new NotImplementedException();
        }

        public virtual TResponse VerifyIfExists(AbstractMoodleServiceClient client, string filter)
        {
            throw new NotImplementedException();
        }

        public virtual ParallelSendResult<TData, TResponse> SendAll()
        {
            throw new NotImplementedException();
        }
    }
}
