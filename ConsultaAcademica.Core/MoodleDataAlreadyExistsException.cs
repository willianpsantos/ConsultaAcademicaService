using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class MoodleDataAlreadyExistsException : Exception
    {
       
        public MoodleDataAlreadyExistsException() : base()
        {

        }

        public MoodleDataAlreadyExistsException(string message) : base(message)
        {

        }
    }
}
