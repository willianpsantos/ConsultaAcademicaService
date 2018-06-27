using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ConsultaAcademica.MoodleClient.Response
{
    public class GetEnrolmentsByUserIdResponse
    {
        [JsonProperty("roleid")]
        public int RoleId { get; set; }

        [JsonProperty("rolename")]
        public string RoleName { get; set; }

        [JsonProperty("userid")]
        public long UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("categoryid")]
        public long CategoryId { get; set; }
        
        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }

        [JsonProperty("courseid")]
        public long CourseId { get; set; }

        [JsonProperty("coursename")]
        public string CourseName { get; set; }
    }
}
