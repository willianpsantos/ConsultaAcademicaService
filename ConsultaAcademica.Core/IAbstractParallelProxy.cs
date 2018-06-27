using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public interface IAbstractParallelProxy<TData, TResponse> where TData : class, new() where TResponse : class, new()
    {
        IEnumerable<TData> GetData(string filter, bool active = true);

        TResponse VerifyIfExists(AbstractMoodleServiceClient client, string filter);

        TResponse SendItem(AbstractMoodleServiceClient client, TData item);

        ParallelSendResult<TData, TResponse> SendAll();
    }
}
