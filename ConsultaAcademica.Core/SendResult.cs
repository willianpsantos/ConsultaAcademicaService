using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class SendResult<TData, TResult>
    {
        public virtual Queue<ImportedResult<TData, TResult>> ImportedSuccessfully { get; set; }

        public virtual Queue<NotImportedReason<TData>> NotImported { get; set; }

        public SendResult()
        {
            ImportedSuccessfully = new Queue<ImportedResult<TData, TResult>>();
            NotImported = new Queue<NotImportedReason<TData>>();
        }
    }
}
