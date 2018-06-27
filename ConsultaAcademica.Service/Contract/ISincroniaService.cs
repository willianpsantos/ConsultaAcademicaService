using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Service.Contract
{
    public interface ISincroniaService : IAbstractService
    {
        IEnumerable<AlunoCodigo> GetAlunosParaSincronizar();

        void ResetarAlunosEAD();
    }
}
