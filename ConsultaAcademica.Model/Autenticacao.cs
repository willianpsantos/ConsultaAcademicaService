using ConsultaAcademica.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Model
{
    public class Autenticacao : IAuthentication
    {
        [JsonProperty("authenticated")]
        public bool Autenticado { get; set; }

        [JsonProperty("created")]
        public DateTime CriadoEm { get; set; }

        [JsonProperty("expiration")]
        public DateTime ExpiraEm { get; set; }

        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("message")]
        public string Mensagem { get; set; }
    }
}
