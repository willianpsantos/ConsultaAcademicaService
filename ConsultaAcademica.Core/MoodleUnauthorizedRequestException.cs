using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public class MoodleUnauthorizedRequestException : Exception
    {
        public MoodleUnauthorizedError MoodleErrorData { get; set; }

        public MoodleUnauthorizedRequestException(string url, HttpStatusCode statusCode) : base()
        {
            MoodleErrorData = new MoodleUnauthorizedError()
            {
                Url = url,
                StatusCode = statusCode
            };
        }
    }
}
