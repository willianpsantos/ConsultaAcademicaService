using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetAllUsersClient : MoodleServiceClient<EmptyRequest, UserResponse[]>
    {
        public GetAllUsersClient() : base()
        {
            Tag = "";
            Function = Constantes.GET_ALL_USERS_FUNCTION;
        }

        public GetAllUsersClient(string function, string tag) : base(function, tag)
        {

        }

        public GetAllUsersClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
