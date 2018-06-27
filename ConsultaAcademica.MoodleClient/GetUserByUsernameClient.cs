using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetUserByUsernameClient : MoodleServiceClient<GetByUsernameRequest, UserResponse>
    {
        public GetUserByUsernameClient() : base()
        {
            Tag = Constantes.GET_BY_USERNAME_TAG;
            Function = Constantes.GET_USER_BY_USERNAME_FUNCTION;
        }

        public GetUserByUsernameClient(string function, string tag) : base(function, tag)
        {

        }

        public GetUserByUsernameClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
