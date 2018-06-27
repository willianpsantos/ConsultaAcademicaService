using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public class Constantes
    {
        public const string ALUNO_DISCIPLINAS_TABLE = "AlunoDisciplinaMoodle";
        public const string ATUALIZAR_EMAIL_DOMINIO = "@atualizar.com.br";
        public const int ROLEID_PROFESSOR = 3;
        public const int ROLEID_ALUNO = 5;
        public const int ROLEID_COORDENADOR = 9;
        public const int CONTEXTLEVEL_CURSO = 50;

        public const string ALTERE_SEU_EMAIL_ENDERECO_1 = "altereseuemail@email.com";
        public const string ALTERE_SEU_EMAIL_ENDERECO_2 = "altereseuemail@univag.edu.br";

        public const string CREATE_CATEGORIES_FUNCTION = "core_course_create_categories";
        public const string CATEGORIES_TAG = "categories";        
        
        public const string CREATE_COURSES_FUNCTION = "core_course_create_courses";
        public const string COURSE_TAG = "courses";

        public const string CREATE_USERS_FUNCTION = "core_user_create_users";
        public const string UPDATE_USERS_FUNCTION = "core_user_update_users";
        public const string USERS_TAG = "users";

        public const string CREATE_ENROLMENTS_FUNCTION = "enrol_manual_enrol_users";
        public const string CREATE_UNENROLMENTS_FUNCTION = "enrol_manual_unenrol_users";
        public const string ENROLMENTS_TAG = "enrolments";

        public const string GET_BY_ID_TAG = "id";
        public const string GET_BY_NAME_TAG = "name";
        public const string GET_BY_USERNAME_TAG = "username";

        public const string GET_CATEGORY_BY_ID_FUNCTION = "local_get_info_api_external_get_category_by_id";
        public const string GET_CATEGORY_BY_NAME_FUNCTION = "local_get_info_api_external_get_category_by_name";
        public const string GET_COURSE_BY_ID_FUNCTION = "local_get_info_api_external_get_course_by_id";
        public const string GET_COURSE_BY_NAME_FUNCTION = "local_get_info_api_external_get_course_by_name";
        public const string GET_USER_BY_ID_FUNCTION = "local_get_info_api_external_get_user_by_id";
        public const string GET_USER_BY_NAME_FUNCTION = "local_get_info_api_external_get_user_by_name";
        public const string GET_USER_BY_USERNAME_FUNCTION = "local_get_info_api_external_get_user_by_username";
        public const string GET_ENROLMENTS_BY_USERID = "local_get_info_api_external_get_enrolments_by_userid";
        public const string GET_ALL_USERS_FUNCTION = "local_get_info_api_external_get_all_users";
        public const string GET_ALL_COURSES_FUNCTION = "local_get_info_api_external_get_all_courses";
        public const string GET_ALL_CATEGORIES_FUNCTION = "local_get_info_api_external_get_all_categories";

        public const string MOODLE_URL_SETTING = "moodle.service.url";
        public const string MOODLE_DOMAIN_SETTING = "moodle.service.domain";
        public const string MOODLE_TOKEN_SETTING = "moodle.service.token";

        public const string PERIODO_LETIVO_PARAMETRO = "PERIODO-LETIVO-ATUAL";
        public const string TEMPO_EXECUCAO_SERVICO_PARAMETRO = "TEMPO-VERIFICACAO-EXECUCACAO-SERVICO";
        public const string CATEGORIA_PAI_PADRAO_PARAMETRO = "CATEGORIA-PAI-PADRAO";
        public const string CATEGORY_DESCRIPTION_FORMAT_PARAMETRO = "CURSO-FORMATO-DESCRICAO-PADRAO";
        public const string GERA_LOG_DOS_JA_ADICIONADOS_PARAMETRO = "GERA-LOG-DOS-JA-ADICIONADOS";
        public const string QTDE_REGISTROS_PAGINACAO_DADOS_PARAMETRO = "QTDE-REGISTROS-PAGINACAO-DADOS";
        public const string ARQUIVO_HTML_EMAIL_PROFESSOR_DADOS_PARAMETRO = "ARQUIVO-HTML-EMAIL-PROFESSOR";
        public const string ARQUIVO_HTML_EMAIL_ALUNO_DADOS_PARAMETRO = "ARQUIVO-HTML-EMAIL-ALUNO";
        public const string ENVIA_EMAIL_ALUNO_PARAMETRO = "ENVIA-EMAIL-ALUNO";
        public const string ENVIA_EMAIL_PROFESSOR_PARAMETRO = "ENVIA-EMAIL-PROFESSOR";
    }
}
