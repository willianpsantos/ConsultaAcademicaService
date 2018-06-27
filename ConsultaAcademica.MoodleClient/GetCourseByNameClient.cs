using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetCourseByNameClient : MoodleServiceClient<GetByNameRequest, CourseResponse>
    {
        public GetCourseByNameClient() : base()
        {
            Tag = Constantes.GET_BY_NAME_TAG;
            Function = Constantes.GET_COURSE_BY_NAME_FUNCTION;
        }

        public GetCourseByNameClient(string function, string tag) : base(function, tag)
        {

        }

        public GetCourseByNameClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
