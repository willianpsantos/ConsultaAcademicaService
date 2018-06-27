using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class NotImportedReason<T>
    {
        public DateTime Date { get; set; }

        public T Data { get; set; }

        public string Url { get; set; }

        public string Reason { get; set; }

        public Exception Exception { get; set; }

        public NotImportedReason()
        {
            Date = DateTime.Now;            
        }
    }
}
