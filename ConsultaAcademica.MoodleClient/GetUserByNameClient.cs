using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetUserByNameClient : MoodleServiceClient<GetByIdRequest, UserResponse>
    {
        public GetUserByNameClient() : base()
        {
            Tag = Constantes.GET_BY_NAME_TAG;
            Function = Constantes.GET_USER_BY_NAME_FUNCTION;
        }

        public GetUserByNameClient(string function, string tag) : base(function, tag)
        {

        }

        public GetUserByNameClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
