using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsultaAcademica.MoodleClient.Response
{
    public class CourseResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("shortname")]
        public string Shortname { get; set; }

        [JsonProperty("category")]
        public int Category { get; set; }
    }
}
