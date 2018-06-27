using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetAllCategoriesClient : MoodleServiceClient<EmptyRequest, CategoryResponse[]>
    {
        public GetAllCategoriesClient() : base()
        {
            Tag = "";
            Function = Constantes.GET_ALL_CATEGORIES_FUNCTION;
        }

        public GetAllCategoriesClient(string function, string tag) : base(function, tag)
        {

        }

        public GetAllCategoriesClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
