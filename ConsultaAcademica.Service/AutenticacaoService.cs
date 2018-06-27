using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Model;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ConsultaAcademica.Service.Contract;
using ConsultaAcademica.Core;

namespace ConsultaAcademica.Service
{
    public class AutenticacaoService : AbstractService, IAutenticacaoService
    {
        public AutenticacaoService() : base()
        {
            ServiceName = "AutenticacaoService";
        }

        public AutenticacaoService(string baseUrl) : base(baseUrl)
        {
            ServiceName = "AutenticacaoService";
        }

        public IAuthentication Autenticar(string cpf, string senha)
        {
            Task<string> task = Post(
                "Token/Authorize", 
                new Dictionary<string, string>()
                {
                    { "Login", cpf },
                    { "Senha", senha }
                }
            );

            task.Wait();

            string content = task.Result;
            Autenticacao autenticacao = JsonConvert.DeserializeObject<Autenticacao>(content);
            return autenticacao;
        }
    }
}
