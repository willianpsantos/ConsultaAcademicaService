using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public interface IAuthentication
    {
        bool Autenticado { get; set; }
        
        DateTime CriadoEm { get; set; }
        
        DateTime ExpiraEm { get; set; }

        string Token { get; set; }

        string Mensagem { get; set; }        
    }
}
