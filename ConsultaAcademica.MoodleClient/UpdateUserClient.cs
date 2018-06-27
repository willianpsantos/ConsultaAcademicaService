using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;

namespace ConsultaAcademica.MoodleClient
{
    public class UpdateUserClient : MoodleServiceClient<UpdateUserRequest, EmptyResponse>
    {
        public UpdateUserClient() : base()
        {
            Tag = Constantes.USERS_TAG;
            Function = Constantes.UPDATE_USERS_FUNCTION;
        }

        public UpdateUserClient(string function, string tag) : base(function, tag)
        {
            
        }

        public UpdateUserClient(string function, string tag, string operation) : base(function, tag, operation)
        {
            
        }
    }
}
