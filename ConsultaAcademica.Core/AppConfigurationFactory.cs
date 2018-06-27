using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ConsultaAcademica.Core
{
    public class AppConfigurationFactory
    {
        public const string TEMPO_SINCRONIZACAO_CONFIG = "tempo.sincronizacao";

        public const string TEMPO_IMPORTACAO_CONFIG = "tempo.importacao";

        public const string CONSULTA_ACADEMICA_API_URL_CONFIG = "consulta.academica.api.url";

        public const string CONSULTA_ACADEMICA_API_USERNAME = "consulta.academica.api.username";

        public const string CONSULTA_ACADEMICA_API_PASSWORD = "consulta.academica.api.password";

        public const string SEMESTRE_ATUAL_CONFIG = "semestre.atual";


        public const string MOODLE_APOIO_PRESENCIAL_URL_CONFIG = "moodle.apoio.presencial.url";

        public const string MOODLE_SEMI_PRESENCIAL_URL_CONFIG = "moodle.semi.presencial.url";

        public const string MOODLE_EAD_URL_CONFIG = "moodle.ead.url";


        public const string MOODLE_APOIO_PRESENCIAL_SERVICE_URL_CONFIG = "moodle.apoio.presencial.service.url";

        public const string MOODLE_SEMI_PRESENCIAL_SERVICE_URL_CONFIG = "moodle.semi.presencial.service.url";

        public const string MOODLE_EAD_SERVICE_URL_CONFIG = "moodle.ead.service.url";


        public const string MOODLE_APOIO_PRESENCIAL_TOKEN_CONFIG = "moodle.apoio.presencial.token";

        public const string MOODLE_SEMI_PRESENCIAL_TOKEN_CONFIG = "moodle.semi.presencial.token";

        public const string MOODLE_EAD_TOKEN_CONFIG = "moodle.ead.token";


        public const string MOODLE_APOIO_PRESENCIAL_GETINFOSERVICE_TOKEN_CONFIG = "moodle.apoio.presencial.getinfoservice.token";

        public const string MOODLE_SEMI_PRESENCIAL_GETINFOSERVICE_TOKEN_CONFIG = "moodle.semi.presencial.getinfoservice.token";

        public const string MOODLE_EAD_GETINFOSERVICE_TOKEN_CONFIG = "moodle.ead.getinfoservice.token";


        public const string ID_MODALIDADE_APOIO_PRESENCIAL_CONFIG = "id.modalidade.apoio.presencial";

        public const string ID_MOLIDADADE_SEMI_PRESENCIAL_CONFIG = "id.modalidade.semi.presencial";

        public const string ID_MODALIDADE_EAD_CONFIG = "id.modalidade.ead";


        public const string ID_CATEGORIA_PADRAO_APOIO_PRESENCIAL_CONFIG = "id.categoria.padrao.apoio.presencial";

        public const string ID_CATEGORIA_PADRAO_SEMI_PRESENCIAL_CONFIG = "id.categoria.padrao.semi.presencial";

        public const string ID_CATEGORIA_PADRAO_EAD_CONFIG = "id.categoria.padrao.ead";
        

        public const string DESCRIPTION_FORMAT_APOIO_PRESENCIAL_CONFIG = "description.format.apoio.presencial";

        public const string DESCRIPTION_FORMAT_SEMI_PRESENCIAL_CONFIG = "description.format.semi.presencial";

        public const string DESCRIPTION_FORMAT_EAD_CONFIG = "description.format.ead";


        private static string GetConfiguration(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        private static void SetConfiguration(ref AppConfiguration config)
        {
            string tempoSincString = GetConfiguration(TEMPO_SINCRONIZACAO_CONFIG);
            double tempoSinc = 0;
            bool sucesso = Double.TryParse(tempoSincString, out tempoSinc);

            if (sucesso)
                config.TempoSincronizacao = tempoSinc;

            string tempoImportacaoString = GetConfiguration(TEMPO_IMPORTACAO_CONFIG);
            double tempoImport = 0;

            sucesso = Double.TryParse(tempoImportacaoString, out tempoImport);

            if (sucesso)
                config.TempoImportacao = tempoImport;

            config.ConsultaAcademicaApiURL = GetConfiguration(CONSULTA_ACADEMICA_API_URL_CONFIG);
            config.ConsultaAcademicaApiUsername = GetConfiguration(CONSULTA_ACADEMICA_API_USERNAME);
            config.ConsultaAcademicaApiPassword = GetConfiguration(CONSULTA_ACADEMICA_API_PASSWORD);
            config.SemestreAtual = GetConfiguration(SEMESTRE_ATUAL_CONFIG);

            config.MoodleApoioPresencialUrl = GetConfiguration(MOODLE_APOIO_PRESENCIAL_URL_CONFIG);
            config.MoodleSemiPresencialUrl = GetConfiguration(MOODLE_SEMI_PRESENCIAL_URL_CONFIG);
            config.MoodleEadUrl = GetConfiguration(MOODLE_EAD_URL_CONFIG);

            config.MoodleApoioPresencialServiceUrl = GetConfiguration(MOODLE_APOIO_PRESENCIAL_SERVICE_URL_CONFIG);
            config.MoodleSemiPresencialServiceUrl = GetConfiguration(MOODLE_SEMI_PRESENCIAL_SERVICE_URL_CONFIG);
            config.MoodleEadServiceUrl = GetConfiguration(MOODLE_EAD_SERVICE_URL_CONFIG);

            config.MoodleApoioPresencialToken = GetConfiguration(MOODLE_APOIO_PRESENCIAL_TOKEN_CONFIG);
            config.MoodleSemiPresencialToken = GetConfiguration(MOODLE_SEMI_PRESENCIAL_TOKEN_CONFIG);
            config.MoodleEadToken = GetConfiguration(MOODLE_EAD_TOKEN_CONFIG);

            config.MoodleApoioPresencialGetInfoServiceToken = GetConfiguration(MOODLE_APOIO_PRESENCIAL_GETINFOSERVICE_TOKEN_CONFIG);
            config.MoodleSemiPresencialGetInfoServiceToken = GetConfiguration(MOODLE_SEMI_PRESENCIAL_GETINFOSERVICE_TOKEN_CONFIG);
            config.MoodleEadGetInfoServiceToken = GetConfiguration(MOODLE_EAD_GETINFOSERVICE_TOKEN_CONFIG);

            config.IdModalidadeApoioPresencial = Convert.ToInt32(GetConfiguration(ID_MODALIDADE_APOIO_PRESENCIAL_CONFIG));
            config.IdModalidadeSemiPresencial = Convert.ToInt32(GetConfiguration(ID_MOLIDADADE_SEMI_PRESENCIAL_CONFIG));
            config.IdModalidadeEad = Convert.ToInt32(GetConfiguration(ID_MODALIDADE_EAD_CONFIG));

            config.IdCategoriaPadraoApoioPresencial = Convert.ToInt32(GetConfiguration(ID_CATEGORIA_PADRAO_APOIO_PRESENCIAL_CONFIG));
            config.IdCategoriaPadraoSemiPresencial = Convert.ToInt32(GetConfiguration(ID_CATEGORIA_PADRAO_SEMI_PRESENCIAL_CONFIG));
            config.IdCategoriaPadraoEad = Convert.ToInt32(GetConfiguration(ID_CATEGORIA_PADRAO_EAD_CONFIG));

            config.DescriptionFormatApoioPresencial = Convert.ToInt32(GetConfiguration(DESCRIPTION_FORMAT_APOIO_PRESENCIAL_CONFIG));
            config.DescriptionFormatSemiPresencial = Convert.ToInt32(GetConfiguration(DESCRIPTION_FORMAT_SEMI_PRESENCIAL_CONFIG));
            config.DescriptionFormatEad = Convert.ToInt32(GetConfiguration(DESCRIPTION_FORMAT_EAD_CONFIG));
        }

        public static AppConfiguration Create()
        {
            AppConfiguration config = new AppConfiguration();
            SetConfiguration(ref config);
            return config;
        }

        public static void Refresh(AppConfiguration config)
        {
            SetConfiguration(ref config);
        }
    }
}
