using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetAllCoursesClient : MoodleServiceClient<EmptyRequest, CourseResponse[]>
    {
        public GetAllCoursesClient() : base()
        {
            Tag = "";
            Function = Constantes.GET_ALL_COURSES_FUNCTION;
        }

        public GetAllCoursesClient(string function, string tag) : base(function, tag)
        {

        }

        public GetAllCoursesClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
