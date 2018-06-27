using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class AppConfiguration
    {
        public double TempoSincronizacao { get; set; }

        public double TempoImportacao { get; set; }

        public string ConsultaAcademicaApiURL { get; set; }

        public string ConsultaAcademicaApiUsername { get; set; }

        public string ConsultaAcademicaApiPassword { get; set; }

        public string SemestreAtual { get; set; }


        public string MoodleApoioPresencialUrl { get; set; }

        public string MoodleSemiPresencialUrl { get; set; }

        public string MoodleEadUrl { get; set; }


        public string MoodleApoioPresencialServiceUrl { get; set; }

        public string MoodleSemiPresencialServiceUrl { get; set; }

        public string MoodleEadServiceUrl { get; set; }


        public string MoodleApoioPresencialToken { get; set; }

        public string MoodleSemiPresencialToken { get; set; }

        public string MoodleEadToken { get; set; }


        public string MoodleApoioPresencialGetInfoServiceToken { get; set; }

        public string MoodleSemiPresencialGetInfoServiceToken { get; set; }

        public string MoodleEadGetInfoServiceToken { get; set; }


        public int IdModalidadeApoioPresencial { get; set; }

        public int IdModalidadeSemiPresencial { get; set; }

        public int IdModalidadeEad { get; set; }


        public int IdCategoriaPadraoApoioPresencial { get; set; }

        public int IdCategoriaPadraoSemiPresencial { get; set; }

        public int IdCategoriaPadraoEad { get; set; }


        public int DescriptionFormatApoioPresencial { get; set; }

        public int DescriptionFormatSemiPresencial { get; set; }

        public int DescriptionFormatEad { get; set; }
    }
}
