using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetCategoryByNameClient : MoodleServiceClient<GetByNameRequest, CategoryResponse>
    {
        public GetCategoryByNameClient() : base()
        {
            Tag = Constantes.GET_BY_NAME_TAG;
            Function = Constantes.GET_CATEGORY_BY_NAME_FUNCTION;
        }

        public GetCategoryByNameClient(string function, string tag) : base(function, tag)
        {

        }

        public GetCategoryByNameClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
