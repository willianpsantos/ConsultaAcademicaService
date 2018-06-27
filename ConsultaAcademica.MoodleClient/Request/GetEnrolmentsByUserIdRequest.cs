using System;
using System.Collections.Generic;
using System.Text;
using ConsultaAcademica.Core;
using Newtonsoft.Json;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(MoodleUrlDataConvertType.AsValue)]
    public class GetEnrolmentsByUserIdRequest
    {
        [JsonProperty("userid")]
        public long UserId { get; set; }

        [JsonProperty("roleid")]
        public int RoleId { get; set; }

        [JsonProperty("categoryid")]
        public long CategoryId { get; set; }

        [JsonProperty("courseid")]
        public long CourseId { get; set; }

        public GetEnrolmentsByUserIdRequest()
        {
            UserId = 0;
            RoleId = 0;
            CategoryId = 0;
            CourseId = 0;
        }
    }
}
