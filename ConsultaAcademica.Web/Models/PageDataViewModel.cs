using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaAcademica.Web.Models
{
    public class PageDataViewModel<T>
    {
        public FilterType FilterType { get; set; }

        public string FilteValue { get; set; }

        public string AditionalFilter { get; set; }

        public int Total { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
