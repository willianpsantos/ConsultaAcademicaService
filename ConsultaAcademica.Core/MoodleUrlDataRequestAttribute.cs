using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public enum MoodleUrlDataConvertType
    {
        AsArray,
        AsValue
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MoodleUrlDataRequestAttribute : Attribute
    {
        public string Tag { get; set; }

        public MoodleUrlDataConvertType ConvertType { get; set; }

        public MoodleUrlDataRequestAttribute()
        {
            
        }

        public MoodleUrlDataRequestAttribute(MoodleUrlDataConvertType convertType)
        {
            Tag = "";
            ConvertType = convertType;
        }

        public MoodleUrlDataRequestAttribute(string tag)
        {
            Tag = tag;
            ConvertType = MoodleUrlDataConvertType.AsArray;
        }

        public MoodleUrlDataRequestAttribute(string tag, MoodleUrlDataConvertType convertType)
        {
            Tag = tag;
            ConvertType = convertType;
        }
    }
}
