using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public static class EnumExtensions
    {
        public static object GetDbEnumValue(this Enum @enum)
        {
            var type = @enum.GetType();
            var field = type.GetField(@enum.ToString());
            var attrs = field.GetCustomAttributes(typeof(EnumValueAttribute), false);

            if(attrs?.Length == 0)
            {
                return default(object);
            }

            var attr = (EnumValueAttribute)attrs[0];
            return attr.Value;
        }
    }
}
