using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;

namespace ConsultaAcademica.MoodleClient
{
    public class CreateCourseClient : MoodleServiceClient<CourseRequest, CourseResponse>
    {
        public CreateCourseClient() : base()
        {
            Tag = Constantes.COURSE_TAG;
            Function = Constantes.CREATE_COURSES_FUNCTION;
        }

        public CreateCourseClient(string function, string tag) : base(function, tag)
        {
            
        }

        public CreateCourseClient(string function, string tag, string operation) : base(function, tag, operation)
        {
            
        }
    }
}
