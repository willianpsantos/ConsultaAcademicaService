using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public interface IAbstractService
    {
        string ServiceName { get; set; }

        string Token { get; set; }
        
        string BaseUrl { get; set; }
    }
}
