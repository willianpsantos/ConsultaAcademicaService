using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetCategoryByIdClient : MoodleServiceClient<GetByIdRequest, CategoryResponse>
    {
        public GetCategoryByIdClient() : base()
        {
            Tag = Constantes.GET_BY_ID_TAG;
            Function = Constantes.GET_CATEGORY_BY_ID_FUNCTION;
        }

        public GetCategoryByIdClient(string function, string tag) : base(function, tag)
        {

        }

        public GetCategoryByIdClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
