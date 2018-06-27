using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Service
{
    public class SincroniaService : AbstractService, ISincroniaService
    {
        private readonly string _ApiServiceName = "Sincronia";

        public SincroniaService() : base()
        {
            ServiceName = "SincroniaService";
        }

        public SincroniaService(string baseUrl) : base(baseUrl)
        {
            ServiceName = "SincroniaService";
        }

        public IEnumerable<AlunoCodigo> GetAlunosParaSincronizar()
        {
            Task<string> task = Get($"{_ApiServiceName}/ListarAlunosEAD", null);
            task.Wait();

            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new AlunoCodigo[] { };
            }

            AlunoCodigo[] codigos = JsonConvert.DeserializeObject<AlunoCodigo[]>(content);
            return codigos;
        }

        public void ResetarAlunosEAD()
        {
            Task<string> task = Delete($"{_ApiServiceName}/ResetarAlunosEAD", null);
            task.Wait();

            string content = task.Result;
        }
    }
}
