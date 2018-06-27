using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.MoodleClient;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using ConsultaAcademica.Proxy.Result;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsultaAcademica.Proxy
{
    public static class ProxyExtensions
    {
        private static long? InternalGetMoodleCursoId
        (            
            string cursoDescricao, 
            Modalidade modalidade, 
            GetCategoryByNameClient client = null, 
            HttpClient httpClient = null
        )
        {
            long? moodleCategoryId = MoodleFromToCache.GetCachedMoodleCategory(modalidade.IdModalidade, cursoDescricao);

            if (moodleCategoryId.HasValue)
            {
                return moodleCategoryId.Value;
            }

            GetCategoryByNameClient getClient = client;

            if (getClient == null)
            {
                getClient = new GetCategoryByNameClient();
            }

            if(httpClient != null)
            {
                getClient.AddHttpClient(httpClient);
            }

            getClient.AddBaseUrl(modalidade.MoodleUrl)
                     .AddServiceUrl(modalidade.MoodleServiceUrl)
                     .AddToken(modalidade.MoodleGetInfoServiceToken);

            GetByNameRequest request = new GetByNameRequest()
            {
                Name = cursoDescricao
            };

            Task<CategoryResponse> task = getClient.Post(request);
            task.Wait();

            CategoryResponse response = task.Result;

            if(response?.Id > 0)
            {
                MoodleFromToCache.AddCategory(modalidade.IdModalidade, cursoDescricao, response.Id);
            }

            return response?.Id;

        }

        private static long? InternalGetMoodleUserId
        (            
            string cpf,
            Modalidade modalidade,
            GetUserByUsernameClient client = null,
            HttpClient httpClient = null
        )
        {
            long? moodleUserId = MoodleFromToCache.GetCachedMoodleUser(modalidade.IdModalidade, cpf);

            if (moodleUserId.HasValue)
            {
                return moodleUserId.Value;
            }

            GetUserByUsernameClient getClient = client;

            if (getClient == null)
            {
                getClient = new GetUserByUsernameClient();
            }

            if (httpClient != null)
            {
                getClient.AddHttpClient(httpClient);
            }

            getClient.AddBaseUrl(modalidade.MoodleUrl)
                     .AddServiceUrl(modalidade.MoodleServiceUrl)
                     .AddToken(modalidade.MoodleGetInfoServiceToken);

            GetByUsernameRequest request = new GetByUsernameRequest()
            {
                Username = cpf.DesformatarCpf()
            };

            Task<UserResponse> task = getClient.Post(request);
            task.Wait();

            UserResponse response = task.Result;

            if (response?.Id > 0)
            {
                MoodleFromToCache.AddUser(modalidade.IdModalidade, cpf, response.Id);
            }

            return response?.Id;
        }

        public static string GetNomeDisciplina
        (
            this Disciplina disciplina, 
            AppConfiguration configuration, 
            Modalidade modalidade
        )
        {
            if (modalidade.IdModalidade == configuration.IdModalidadeApoioPresencial)
            {
                char turnoSigla = disciplina.TurnoDescricao[0];
                return $"{disciplina.DisciplinaNome} ( {disciplina.TurmaSigla}-{turnoSigla} )";
            }

            return disciplina.DisciplinaNome;
        }

        public static string GetShortNameDisciplina(this Disciplina disciplina, AppConfiguration configuration, Modalidade modalidade)
        {
            if (modalidade.IdModalidade == configuration.IdModalidadeApoioPresencial)
            {
                var shortname = $"{disciplina.IdDisciplina}-{disciplina.DisciplinaSigla}-{disciplina.TurmaSigla}-{disciplina.TurnoDescricao.Substring(0, 1)}";
                return shortname.ToUpper();
            }

            return disciplina.DisciplinaSigla.ToUpper();
        }

        public static long? GetMoodleCursoId
        (
            this Disciplina item, 
            Modalidade modalidade, 
            GetCategoryByNameClient client = null, 
            HttpClient httpClient = null
        )
        {
            return InternalGetMoodleCursoId(item.CursoDescricao, modalidade, client, httpClient);
        }

        public static long? GetMoodleCursoId
        (
            this Curso item, 
            Modalidade modalidade, 
            GetCategoryByNameClient client = null, 
            HttpClient httpClient = null
        )
        {
            return InternalGetMoodleCursoId(item.CursoDescricao, modalidade, client, httpClient);
        }

        public static long? GetMoodleDisciplinaId
        (
            this Disciplina item, 
            AppConfiguration configuration, 
            Modalidade modalidade, 
            GetCourseByNameClient client = null, 
            HttpClient httpClient = null
        )
        {
            string disciplinaNome = GetNomeDisciplina(item, configuration, modalidade);
            long? moodleDisciplineId = MoodleFromToCache.GetCachedMoodleCourse(modalidade.IdModalidade, disciplinaNome);

            if (moodleDisciplineId.HasValue)
            {
                return moodleDisciplineId.Value;
            }
            
            GetCourseByNameClient getClient = client;

            if (getClient == null)
            {
                getClient = new GetCourseByNameClient();
            }

            if (httpClient != null)
            {
                getClient.AddHttpClient(httpClient);
            }

            getClient.AddBaseUrl(modalidade.MoodleUrl)
                     .AddServiceUrl(modalidade.MoodleServiceUrl)
                     .AddToken(modalidade.MoodleGetInfoServiceToken);

            GetByNameRequest request = new GetByNameRequest()
            {
                Name = item.DisciplinaNome
            };

            Task<CourseResponse> task = getClient.Post(request);
            task.Wait();

            CourseResponse response = task.Result;

            if(response?.Id > 0)
            {
                MoodleFromToCache.AddCourse(modalidade.IdModalidade, disciplinaNome, response.Id);
            }

            return response?.Id;
        }

        public static long? GetMoodleUserId
        (
            this Aluno item, 
            Modalidade modalidade,
            GetUserByUsernameClient client = null,
            HttpClient httpClient = null
        )
        {
            return InternalGetMoodleUserId(item.AlunoCpf, modalidade, client, httpClient);
        }

        public static long? GetMoodleUserId
        (
            this Professor item,
            Modalidade modalidade,
            GetUserByUsernameClient client = null,
            HttpClient httpClient = null
        )
        {
            return InternalGetMoodleUserId(item.ProfessorCpf, modalidade, client, httpClient);
        }

        public static string TratarEmail(this string email, string matricula)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            {
                return $"{matricula}{Constantes.ATUALIZAR_EMAIL_DOMINIO}";
            }

            var tempEmail = email.Trim().Replace(" ", "");
            var altereSeuEmail1 = Constantes.ALTERE_SEU_EMAIL_ENDERECO_1.Trim().Replace(" ", "");
            var altereSeuEmail2 = Constantes.ALTERE_SEU_EMAIL_ENDERECO_2.Trim().Replace(" ", "");

            if (string.IsNullOrEmpty(tempEmail) || 
                 string.IsNullOrWhiteSpace(tempEmail) || 
                  tempEmail == altereSeuEmail1 || 
                   tempEmail == altereSeuEmail2)
            {
                return $"{matricula}{Constantes.ATUALIZAR_EMAIL_DOMINIO}";
            }

            var regex = @"^[\w.-]+@[\w.-]+\.[a-zA-Z]{2,6}$";

            if (!Regex.IsMatch(tempEmail, regex))
            {
                return tempEmail;
            }

            return tempEmail;
        }

        public static SuspendedUserResult SuspendItem(this AbstractProxy proxy, object item, Modalidade modalidade, HttpClient httpClient)
        {
            SuspendedUserResult suspendedResult = new SuspendedUserResult()
            {
                MoodleId = null,
                LastUrl = ""
            };

            UpdateUserClient client = new UpdateUserClient();
            GetUserByUsernameClient getUserClient = new GetUserByUsernameClient();

            client.AddHttpClient(httpClient);
            getUserClient.AddHttpClient(httpClient);

            proxy.BuildMoodleClient(client, MoodleTokenType.OfficialMoodleApiFunctions);
            proxy.BuildMoodleClient(getUserClient, MoodleTokenType.LocalMoodleExternalApiGetInfoToken);

            long? moodleId = null;

            if (item is Professor professor)
            {
                moodleId = professor?.GetMoodleUserId(modalidade, getUserClient, httpClient);
            }
            else if (item is Aluno aluno)
            { 
                moodleId = aluno?.GetMoodleUserId(modalidade, getUserClient, httpClient);
            }

            if (!moodleId.HasValue)
            {
                return suspendedResult;
            }

            UpdateUserRequest request = new UpdateUserRequest()
            {
                Id = moodleId.Value,
                Suspended = 1
            };

            Task<EmptyResponse> task = client.Post(request);
            task.Wait();

            suspendedResult.LastUrl = client.LastUrl;
            suspendedResult.MoodleId = moodleId;
            return suspendedResult;
        }
    }
}
