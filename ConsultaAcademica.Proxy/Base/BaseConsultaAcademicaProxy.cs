using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Proxy
{
    public abstract class BaseConsultaAcademicaProxy<TData, TResponse> : AbstractProxy, IAbstractProxy<TData, TResponse>
        where TData : class, new() 
        where TResponse : class, new()
    {
        protected IConsultaAcademicaService ConsultaAcademicaService;
        protected IEnumerable<Modalidade> Modalidades;
        protected string SemestreAtual;

        public BaseConsultaAcademicaProxy(IAbstractService abstractService) : base(abstractService)
        {
            ConsultaAcademicaService = (IConsultaAcademicaService)Service;
        }

        public BaseConsultaAcademicaProxy<TData, TResponse> AddModalidades(IEnumerable<Modalidade> modalidades)
        {
            Modalidades = modalidades;
            return this;
        }

        public BaseConsultaAcademicaProxy<TData, TResponse> AddSemestreAtual(string semestre)
        {
            SemestreAtual = semestre;
            return this;
        }

        public virtual IEnumerable<TData> GetData(string filter, bool active = true)
        {
            throw new NotImplementedException();
        }

        public virtual SendResult<TData, TResponse> SendAll()
        {
            throw new NotImplementedException();
        }

        public virtual TResponse SendItem(TData item)
        {
            throw new NotImplementedException();
        }

        public virtual TResponse VerifyIfExists(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
