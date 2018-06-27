using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using ConsultaAcademica.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.MoodleClient
{
    public class UnenrolmentClient : MoodleServiceClient<EnrolmentRequest, EmptyResponse>
    {
        public UnenrolmentClient() : base()
        {
            Tag = Constantes.ENROLMENTS_TAG;
            Function = Constantes.CREATE_UNENROLMENTS_FUNCTION;
        }

        public UnenrolmentClient(string function, string tag) : base(function, tag)
        {

        }

        public UnenrolmentClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
