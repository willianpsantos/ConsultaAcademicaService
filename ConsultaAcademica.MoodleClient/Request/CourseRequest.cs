using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsultaAcademica.Core;
using Newtonsoft.Json;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(Constantes.COURSE_TAG, MoodleUrlDataConvertType.AsArray)]
    public class CourseRequest
    {
        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("shortname")]
        public string Shortname { get; set; }

        [JsonProperty("categoryid")]
        public long CategoryId { get; set; }

        [JsonProperty("visible")]
        public short Visible { get; set; }
    }
}
