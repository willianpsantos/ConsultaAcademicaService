using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public class Email
    {
        public string To { get; set; }

        public string Username { get; set; }

        public string Subject { get; set; }

        public bool BodyIsHtmlFile { get; set; }

        public string Body { get; set; }

        public dynamic Data { get; set; }
    }
}
