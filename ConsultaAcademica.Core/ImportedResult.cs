using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class ImportedResult<TData, TResult>
    {
        public DateTime Date { get; set; }

        public string Url { get; set; }

        public TData Data { get; set; }

        public TResult Result { get; set; }

        public bool Active { get; set; }

        public ImportedResult()
        {
            Active = true;
        }
    }
}
