using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumValueAttribute : Attribute
    {
        public object Value { get; set; }

        public EnumValueAttribute(object value)
        {
            Value = value;
        }
    }
}
