using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ConsultaAcademica.Core;

namespace ConsultaAcademica.MoodleClient.Request
{
    [MoodleUrlDataRequest(Constantes.CATEGORIES_TAG, MoodleUrlDataConvertType.AsArray)]
    public class CategoryRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parent")]
        public int Parent { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("descriptionformat")]
        public int DescriptionFormat { get; set; }

        public CategoryRequest()
        {
            
        }
    }
}
