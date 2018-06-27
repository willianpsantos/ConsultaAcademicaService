using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetUserByIdClient : MoodleServiceClient<GetByIdRequest, UserResponse>
    {
        public GetUserByIdClient() : base()
        {
            Tag = Constantes.GET_BY_ID_TAG;
            Function = Constantes.GET_USER_BY_ID_FUNCTION;
        }

        public GetUserByIdClient(string function, string tag) : base(function, tag)
        {

        }

        public GetUserByIdClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
