using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetEnrolementsrByUserIdClient : MoodleServiceClient<GetEnrolmentsByUserIdRequest, GetEnrolmentsByUserIdResponse>
    {
        public GetEnrolementsrByUserIdClient() : base()
        {
            Tag = Constantes.GET_BY_ID_TAG;
            Function = Constantes.GET_ENROLMENTS_BY_USERID;
        }

        public GetEnrolementsrByUserIdClient(string function, string tag) : base(function, tag)
        {

        }

        public GetEnrolementsrByUserIdClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
