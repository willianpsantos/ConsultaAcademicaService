using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Importador
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            var service = new ConsultaAcademicaImportadorService();
            service.AutoLog = true;
            service.CanHandlePowerEvent = true;
            service.CanShutdown = true;
            service.CanStop = true;
            service.CanPauseAndContinue = true;                      

            ServicesToRun = new ServiceBase[]
            {
                service
            };
            
            ServiceBase.Run(ServicesToRun);
        }
    }
}
