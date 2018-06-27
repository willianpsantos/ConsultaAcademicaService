using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public interface IAbstractProxy<TData, TResponse> where TData : class, new() where TResponse : class, new()
    {
        IEnumerable<TData> GetData(string filter, bool active = true);

        TResponse VerifyIfExists(string filter);

        TResponse SendItem(TData item);

        SendResult<TData, TResponse> SendAll();
    }
}
