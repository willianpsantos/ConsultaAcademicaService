using ConsultaAcademica.Core;
using ConsultaAcademica.Service.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ConsultaAcademica.Model;
using ConsultaAcademica.MoodleClient;
using System.Threading.Tasks;
using ConsultaAcademica.MoodleClient.Response;
using System.Net.Http;

namespace ConsultaAcademica.Proxy.DataCache
{
    public class MoodleFromToCacheAdapter
    {
        private IConsultaAcademicaService Service;
        private AppConfiguration Configuration;
        private IEnumerable<Modalidade> Modalidades;


        public MoodleFromToCacheAdapter(IConsultaAcademicaService service)
        {
            Service = service;            
        }


        public MoodleFromToCacheAdapter AddConfiguration(AppConfiguration config)
        {
            Configuration = config;
            return this;
        }

        public MoodleFromToCacheAdapter AddModalidades(IEnumerable<Modalidade> modalidades)
        {
            Modalidades = modalidades;
            return this;
        }

        private IEnumerable<CategoryResponse> GetMoodleCategories(Modalidade modalidade, HttpClient httpClient)
        {
            GetAllCategoriesClient client = new GetAllCategoriesClient();
            EmptyRequest request = new EmptyRequest();

            Task<CategoryResponse[]> task = client.AddHttpClient(httpClient)
                                                  .AddServiceUrl(modalidade.MoodleServiceUrl)
                                                  .AddBaseUrl(modalidade.MoodleUrl)
                                                  .AddToken(modalidade.MoodleGetInfoServiceToken)
                                                  .Post(request);
            task.Wait();

            CategoryResponse[] response = task.Result;
            return response;
        }

        private IEnumerable<UserResponse> GetMoodleUsers(Modalidade modalidade, HttpClient httpClient)
        {
            GetAllUsersClient client = new GetAllUsersClient();
            EmptyRequest request = new EmptyRequest();

            Task<UserResponse[]> task = client.AddHttpClient(httpClient)
                                              .AddServiceUrl(modalidade.MoodleServiceUrl)
                                              .AddBaseUrl(modalidade.MoodleUrl)
                                              .AddToken(modalidade.MoodleGetInfoServiceToken)
                                              .Post(request);
            task.Wait();

            UserResponse[] response = task.Result;
            return response;

        }

        private IEnumerable<CourseResponse> GetMoodleCourses(Modalidade modalidade, HttpClient httpClient)
        {
            GetAllCoursesClient client = new GetAllCoursesClient();
            EmptyRequest request = new EmptyRequest();

            Task<CourseResponse[]> task = client.AddHttpClient(httpClient)
                                                .AddServiceUrl(modalidade.MoodleServiceUrl)
                                                .AddBaseUrl(modalidade.MoodleUrl)
                                                .AddToken(modalidade.MoodleGetInfoServiceToken)
                                                .Post(request);
            task.Wait();

            CourseResponse[] response = task.Result;
            return response;
        }

        public MoodleFromToCacheAdapter FillCategories()
        {            
            var cursos = Service.GetCursosBySemestre(Configuration.SemestreAtual);

            if (cursos == null)
            {
                return this;
            }

            
            var factory = new HttpClientFactory();

            using (var httpClient = factory.CreateMoodleHttpClient())
            {                
                var getCategoryClient = new GetCategoryByNameClient();

                // Sharing the same HttpClient instance to improve performance
                getCategoryClient.AddHttpClient(httpClient);

                foreach (var modalidade in Modalidades)
                {
                    IEnumerable<CategoryResponse> moodleCategories = GetMoodleCategories(modalidade, httpClient);
                    

                    if(moodleCategories?.Count() == 0)
                    {
                        continue;
                    }

                    getCategoryClient
                        .AddBaseUrl(modalidade.MoodleUrl)
                        .AddToken(modalidade.MoodleGetInfoServiceToken)
                        .AddServiceUrl(modalidade.MoodleServiceUrl);

                    var filtered = cursos.Where(x => x.IdModalidade == modalidade.IdModalidade).ToArray();

                    filtered.AsParallel()
                            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                            .ForAll((item) =>
                            {
                                CategoryResponse moodleCategory = moodleCategories.Where(x => x.Name.ToLower() == item.CursoDescricao.ToLower())
                                                                                  .FirstOrDefault();

                                if(moodleCategory != null)
                                {
                                    MoodleFromToCache.AddCategory(modalidade.IdModalidade, item.CursoDescricao, moodleCategory.Id);                                 
                                }
                            });
                }
            }

            return this;
        }

        public MoodleFromToCacheAdapter FillAlumns()
        {            
            var alunos = Service.GetAlunosBySemestre(Configuration.SemestreAtual);

            if (alunos == null)
            {
                return this;
            }
            
            var factory = new HttpClientFactory();

            using (var httpClient = factory.CreateMoodleHttpClient())
            {
                var getUserClient = new GetUserByUsernameClient();

                // Sharing the same HttpClient instance to improve performance
                getUserClient.AddHttpClient(httpClient);

                foreach (var modalidade in Modalidades)
                {
                    var moodleUsers = GetMoodleUsers(modalidade, httpClient);

                    if (moodleUsers?.Count() == 0)
                    {
                        continue;
                    }

                    getUserClient.AddBaseUrl(modalidade.MoodleUrl)
                                 .AddToken(modalidade.MoodleGetInfoServiceToken)
                                 .AddServiceUrl(modalidade.MoodleServiceUrl);

                    var filtered = alunos.Where(x => x.IdModalidade == modalidade.IdModalidade).ToArray();
                    var total = filtered.Length;
                    var rowsPerPage = 2000;

                    if (total <= rowsPerPage)
                    {
                        filtered.AsParallel()
                                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                .ForAll((item) =>
                                {
                                    var moodleUser = moodleUsers.Where(x => x.Username == item.AlunoCpf.DesformatarCpf()).FirstOrDefault();

                                    if(moodleUser != null)
                                    {
                                        MoodleFromToCache.AddUser(modalidade.IdModalidade, item.AlunoCpf, moodleUser.Id);                                     
                                    }
                                });
                    }
                    else
                    {
                        double div = total / rowsPerPage;
                        var pages = (int)(Math.Floor(div));
                        var rest = total % rowsPerPage;

                        Parallel.For(1, pages + 1, (page) =>                        
                        {
                            var pageable = filtered.Skip((page - 1) * rowsPerPage).Take(rowsPerPage);

                            pageable.AsParallel()
                                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                    .WithDegreeOfParallelism(400)
                                    .ForAll((item) =>
                                    {
                                        var moodleUser = moodleUsers.Where(x => x.Username == item.AlunoCpf.DesformatarCpf()).FirstOrDefault();

                                        if (moodleUser != null)
                                        {
                                            MoodleFromToCache.AddUser(modalidade.IdModalidade, item.AlunoCpf, moodleUser.Id);                                         
                                        }
                                    });
                        });

                        if(rest > 0)
                        {
                            filtered.Skip((pages - 1) * rowsPerPage)
                                    .Take(rest)
                                    .AsParallel()
                                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                    .WithDegreeOfParallelism(400)
                                    .ForAll((item) =>
                                    {
                                        var moodleUser = moodleUsers.Where(x => x.Username == item.AlunoCpf.DesformatarCpf()).FirstOrDefault();

                                        if (moodleUser != null)
                                        {
                                            MoodleFromToCache.AddUser(modalidade.IdModalidade, item.AlunoCpf, moodleUser.Id);                                            
                                        }
                                    });
                        }
                    }
                }
            }

            return this;
        }

        public MoodleFromToCacheAdapter FillTeachers()
        {
            var professores = Service.GetProfessores();

            if (professores == null)
            {
                return this;
            }

            var factory = new HttpClientFactory();

            using (var httpClient = factory.CreateMoodleHttpClient())
            {
                var getUserClient = new GetUserByUsernameClient();

                // Sharing the same HttpClient instance to improve performance
                getUserClient.AddHttpClient(httpClient);

                foreach (var modalidade in Modalidades)
                {
                    var moodleUsers = GetMoodleUsers(modalidade, httpClient);

                    if (moodleUsers?.Count() == 0)
                    {
                        continue;
                    }

                    getUserClient.AddBaseUrl(modalidade.MoodleUrl)
                                 .AddToken(modalidade.MoodleGetInfoServiceToken)
                                 .AddServiceUrl(modalidade.MoodleServiceUrl);

                    var filtered = professores.ToArray();
                    var total = filtered.Length;
                    var rowsPerPage = 2000;

                    if (total <= rowsPerPage)
                    {
                        filtered.AsParallel()
                                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                .ForAll((item) =>
                                {
                                    var moodleUser = moodleUsers.Where(x => x.Username == item.ProfessorCpf.DesformatarCpf()).FirstOrDefault();

                                    if (moodleUser != null)
                                    {
                                        MoodleFromToCache.AddUser(modalidade.IdModalidade, item.ProfessorCpf, moodleUser.Id);
                                    }
                                });
                    }
                    else
                    {
                        double div = total / rowsPerPage;
                        var pages = (int)(Math.Floor(div));
                        var rest = total % rowsPerPage;

                        Parallel.For(1, pages + 1, (page) =>
                        {
                            var pageable = filtered.Skip((page - 1) * rowsPerPage).Take(rowsPerPage);

                            pageable.AsParallel()
                                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                    .WithDegreeOfParallelism(400)
                                    .ForAll((item) =>
                                    {
                                        var moodleUser = moodleUsers.Where(x => x.Username == item.ProfessorCpf.DesformatarCpf()).FirstOrDefault();

                                        if (moodleUser != null)
                                        {
                                            MoodleFromToCache.AddUser(modalidade.IdModalidade, item.ProfessorCpf, moodleUser.Id);
                                        }
                                    });
                        });

                        if (rest > 0)
                        {
                            filtered.Skip((pages - 1) * rowsPerPage)
                                    .Take(rest)
                                    .AsParallel()
                                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                    .WithDegreeOfParallelism(400)
                                    .ForAll((item) =>
                                    {
                                        var moodleUser = moodleUsers.Where(x => x.Username == item.ProfessorCpf.DesformatarCpf()).FirstOrDefault();

                                        if (moodleUser != null)
                                        {
                                            MoodleFromToCache.AddUser(modalidade.IdModalidade, item.ProfessorCpf, moodleUser.Id);
                                        }
                                    });
                        }
                    }
                }
            }

            return this;
        }

        public MoodleFromToCacheAdapter FillCourses()
        {
            var disciplinas = Service.GetDisciplinas(Configuration.SemestreAtual);

            if (disciplinas == null)
            {
                return this;
            }

            var factory = new HttpClientFactory();

            using (var httpClient = factory.CreateMoodleHttpClient())
            {
                var getCourseClient = new GetCourseByNameClient();

                // Sharing the same HttpClient instance to improve performance
                getCourseClient.AddHttpClient(httpClient);

                foreach (var modalidade in Modalidades)
                {
                    var moodleCourses = GetMoodleCourses(modalidade, httpClient);

                    if (moodleCourses?.Count() == 0)
                    {
                        continue;
                    }

                    getCourseClient
                        .AddBaseUrl(modalidade.MoodleUrl)
                        .AddToken(modalidade.MoodleGetInfoServiceToken)
                        .AddServiceUrl(modalidade.MoodleServiceUrl);

                    var filtered = disciplinas.Where(x => x.IdModalidade == modalidade.IdModalidade).ToArray();
                    var total = filtered.Length;
                    var rowsPerPage = 2000;

                    if (total <= rowsPerPage)
                    {
                        filtered.AsParallel()
                                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                .ForAll((item) =>
                                {
                                    item.DisciplinaNome = item.GetNomeDisciplina(Configuration, modalidade);
                                    item.ShortName = item.GetShortNameDisciplina(Configuration, modalidade);

                                    var moodleCourse = moodleCourses.Where(x => x.Fullname == item.DisciplinaNome).FirstOrDefault();

                                    if (moodleCourse != null)
                                    {
                                        MoodleFromToCache.AddCourse(modalidade.IdModalidade, item.DisciplinaNome, moodleCourse.Id);
                                    }
                                });
                    }
                    else
                    {
                        double div = total / rowsPerPage;
                        var pages = (int)(Math.Floor(div));
                        var rest = total % rowsPerPage;

                        Parallel.For(1, pages + 1, (page) =>
                        {
                            var pageable = filtered.Skip((page - 1) * rowsPerPage).Take(rowsPerPage);

                            pageable.AsParallel()
                                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                    .WithDegreeOfParallelism(400)
                                    .ForAll((item) =>
                                    {
                                        item.DisciplinaNome = item.GetNomeDisciplina(Configuration, modalidade);
                                        item.ShortName = item.GetShortNameDisciplina(Configuration, modalidade);

                                        var moodleCourse = moodleCourses.Where(x => x.Fullname == item.DisciplinaNome).FirstOrDefault();

                                        if (moodleCourse != null)
                                        {
                                            MoodleFromToCache.AddCourse(modalidade.IdModalidade, item.DisciplinaNome, moodleCourse.Id);
                                        }
                                    });
                        });

                        if (rest > 0)
                        {
                            filtered.Skip((pages - 1) * rowsPerPage)
                                    .Take(rest)
                                    .AsParallel()
                                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                    .WithDegreeOfParallelism(400)
                                    .ForAll((item) =>
                                    {
                                        item.DisciplinaNome = item.GetNomeDisciplina(Configuration, modalidade);
                                        item.ShortName = item.GetShortNameDisciplina(Configuration, modalidade);

                                        var moodleCourse = moodleCourses.Where(x => x.Fullname == item.DisciplinaNome).FirstOrDefault();

                                        if (moodleCourse != null)
                                        {
                                            MoodleFromToCache.AddCourse(modalidade.IdModalidade, item.DisciplinaNome, moodleCourse.Id);
                                        }
                                    });
                        }
                    }
                }
            }

            return this;
        }
    }
}
