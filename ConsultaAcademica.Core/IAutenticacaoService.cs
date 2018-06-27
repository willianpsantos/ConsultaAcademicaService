using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public interface IAutenticacaoService : IAbstractService
    {
        IAuthentication Autenticar(string cpf, string senha); 
    }
}
