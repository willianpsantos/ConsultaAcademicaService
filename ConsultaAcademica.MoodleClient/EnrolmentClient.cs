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
    public class EnrolmentClient : MoodleServiceClient<EnrolmentRequest, EmptyResponse>
    {
        public EnrolmentClient() : base()
        {
            Tag = Constantes.ENROLMENTS_TAG;
            Function = Constantes.CREATE_ENROLMENTS_FUNCTION;
        }

        public EnrolmentClient(string function, string tag) : base(function, tag)
        {

        }

        public EnrolmentClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
