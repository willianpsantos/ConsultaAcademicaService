using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;

namespace ConsultaAcademica.MoodleClient
{
    public class CreateUserClient : MoodleServiceClient<UserRequest, UserResponse>
    {
        public CreateUserClient() : base()
        {
            Tag = "";
            Function = Constantes.CREATE_USERS_FUNCTION;
        }

        public CreateUserClient(string function, string tag) : base(function, tag)
        {
            
        }

        public CreateUserClient(string function, string tag, string operation) : base(function, tag, operation)
        {
            
        }
    }
}
