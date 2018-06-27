using ConsultaAcademica.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Data.Interfaces
{
    public interface ILogImportacaoRepository<TLogEntity>
    {
        void Save(TLogEntity entity);

        void Save(IEnumerable<TLogEntity> data);
    }
}
