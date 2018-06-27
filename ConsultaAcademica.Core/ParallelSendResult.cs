using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class ParallelSendResult<TData, TResult>
    {
        public ConcurrentQueue<ImportedResult<TData, TResult>> ImportedSuccessfully { get; set; }

        public ConcurrentQueue<NotImportedReason<TData>> NotImported { get; set; }

        public ParallelSendResult()
        {
            ImportedSuccessfully = new ConcurrentQueue<ImportedResult<TData, TResult>>();
            NotImported = new ConcurrentQueue<NotImportedReason<TData>>();
        }
    }
}
