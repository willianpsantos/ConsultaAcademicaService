using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class MoodleDataNotExistsException : Exception
    {
       
        public MoodleDataNotExistsException() : base()
        {

        }

        public MoodleDataNotExistsException(string message) : base(message)
        {

        }
    }
}
