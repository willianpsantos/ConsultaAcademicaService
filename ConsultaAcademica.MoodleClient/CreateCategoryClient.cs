using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class CreateCategoryClient : MoodleServiceClient<CategoryRequest, CategoryResponse>
    {
        public CreateCategoryClient() : base()
        {
            Tag = Constantes.CATEGORIES_TAG;
            Function = Constantes.CREATE_CATEGORIES_FUNCTION;
        }

        public CreateCategoryClient(string function, string tag) : base(function, tag)
        {
            
        }

        public CreateCategoryClient(string function, string tag, string operation) : base(function, tag, operation)
        {
            
        }
    }
}
