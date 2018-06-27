using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Modalidade
    {
        [JsonProperty("IdModalidade")]
        public long IdModalidade { get; set; }

        [JsonProperty("ModalidadeDescricao")]
        public string ModalidadeDescricao { get; set; }

        [JsonIgnore]
        public string MoodleUrl { get; set; }

        [JsonIgnore]
        public string MoodleServiceUrl { get; set; }

        [JsonIgnore]
        public string MoodleToken { get; set; }

        [JsonIgnore]
        public string MoodleGetInfoServiceToken { get; set; }

        [JsonIgnore]
        public int MoodleCategoryParent { get; set; }

        [JsonIgnore]
        public int MoodleDescriptionFormat { get; set; }
    }
}
