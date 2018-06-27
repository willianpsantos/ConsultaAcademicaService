using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ConsultaAcademica.Core;

namespace ConsultaAcademica.Core
{
    public class AbstractServiceBuilder : IDisposable
    {
        private SortedList<string, IAbstractService> _Services;
        private IAutenticacaoService _AuthenticationService;
        private IAuthentication _Authentication;
        private AppConfiguration _Configuration;
        
        public AbstractServiceBuilder(AppConfiguration configuration)
        {
            _Services = new SortedList<string, IAbstractService>();
            _Configuration = configuration;
        }

        public AbstractServiceBuilder AddAuthenticationService(IAutenticacaoService service)
        {
            _AuthenticationService = service;
            return this;
        }

        public AbstractServiceBuilder AddService(IAbstractService service)
        {
            _Services.Add(service.ServiceName, service);
            return this;
        }

        public AbstractServiceBuilder AddConfiguration(AppConfiguration config)
        {
            _Configuration = config;
            return this;
        }

        public T GetService<T>(string serviceName)
        {
            var query = _Services.Where(x => x.Key == serviceName).Select(x => x.Value);

            return (T)query?.FirstOrDefault();
        }

        public void Build()
        {
            _AuthenticationService.BaseUrl = _Configuration.ConsultaAcademicaApiURL;

            _Authentication = _AuthenticationService.Autenticar(
                _Configuration.ConsultaAcademicaApiUsername, 
                _Configuration.ConsultaAcademicaApiPassword
            );

            if(!_Authentication.Autenticado)
            {
                throw new UnauthorizedAccessException("O usuário e senha informados são inválidos");
            }

            var keys = _Services.Keys;

            foreach(var key in keys)
            {
                IAbstractService service = _Services[key];
                service.BaseUrl = _Configuration.ConsultaAcademicaApiURL;
                service.Token = _Authentication.Token;
            }
        }

        public void RefreshToken()
        {
            if(_Authentication == null)
            {
                return;
            }

            if (DateTime.Now <= _Authentication.ExpiraEm)
            {
                return;
            }

            Build();
        }

        public void Dispose()
        {
            _Services.Clear();
            System.GC.Collect();
        }
    }
}
