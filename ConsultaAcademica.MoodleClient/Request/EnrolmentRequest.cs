using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultaAcademica.Core;
using Newtonsoft.Json;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(Constantes.ENROLMENTS_TAG, MoodleUrlDataConvertType.AsArray)]
    public class EnrolmentRequest
    {
        [JsonProperty("roleid")]
        public int RoleId { get; set; }

        [JsonProperty("userid")]
        public long UserId{ get; set; }

        [JsonProperty("courseid")]
        public long CourseId { get; set; }
    }
}
