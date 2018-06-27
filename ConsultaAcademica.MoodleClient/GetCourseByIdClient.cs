using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.MoodleClient
{
    public class GetCourseByIdClient : MoodleServiceClient<GetByIdRequest, CourseResponse>
    {
        public GetCourseByIdClient() : base()
        {
            Tag = Constantes.GET_BY_ID_TAG;
            Function = Constantes.GET_COURSE_BY_ID_FUNCTION;
        }

        public GetCourseByIdClient(string function, string tag) : base(function, tag)
        {

        }

        public GetCourseByIdClient(string function, string tag, string operation) : base(function, tag, operation)
        {

        }
    }
}
