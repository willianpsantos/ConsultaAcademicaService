using ConsultaAcademica.Model;
using ConsultaAcademica.Core;
using ConsultaAcademica.MoodleClient;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ConsultaAcademica.Service.Contract;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

namespace ConsultaAcademica.Proxy
{
    public class ProfessorDisciplinaProxy : BaseParallelConsultaAcademicaProxy<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse>
    {
        private IEnumerable<Professor> Professores;
        private AppConfiguration Configuration;
        private volatile Modalidade ModalidadeAtual;        
        private int MoodleCategoryParent;
        private int MoodleDescriptionFormat;
        
        private HttpClient HttpClient;
        private CreateCourseClient CreateCourseClient;
        private CreateCategoryClient CreateCategoryClient;
        private CreateUserClient CreateUserClient;
        private GetCategoryByNameClient GetCategoryByNameClient;
        private GetCourseByNameClient GetCourseByNameClient;
        private GetUserByUsernameClient GetUserByUsernameClient;
        private EnrolmentClient EnrolmentClient;
        private GetEnrolementsrByUserIdClient GetEnrolementsrByUserIdClient;


        public ProfessorDisciplinaProxy(IConsultaAcademicaService service) : base(service)
        {
            
        }


        public ProfessorDisciplinaProxy AddProfessores(IEnumerable<Professor> professores)
        {
            Professores = professores;
            return this;
        }

        public ProfessorDisciplinaProxy AddConfiguration(AppConfiguration configuration)
        {
            Configuration = configuration;
            return this;
        }

        public ProfessorDisciplinaProxy AddMoodleCategoryParent(int parent)
        {
            MoodleCategoryParent = parent;
            return this;
        }

        public ProfessorDisciplinaProxy AddMoodleDescriptionFormat(int format)
        {
            MoodleDescriptionFormat = format;
            return this;
        }


        private UserResponse CriarUsuarioMoodle(Professor item)
        {
            CreateUserClient client = CreateUserClient;

            BuildMoodleClient(client, MoodleTokenType.OfficialMoodleApiFunctions);

            if (item.ProfessorEmail == null)
                item.ProfessorEmail = "";

            var nomes = item.ProfessorNome.SepararNomeSobrenome();

            UserRequest request = new UserRequest()
            {
                Email = item.ProfessorEmail.TratarEmail(item.ProfessorMatricula),
                Firstname = nomes.Item1,
                Lastname = nomes.Item2,
                Password = item.ProfessorMatricula,
                Username = item.ProfessorCpf.DesformatarCpf()
            };

            request.Preferences.Add("auth_forcepasswordchange", "1");

            Task<UserResponse> task = client.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
            UserResponse response = task.Result;

            if (response != null)
            {
                response.Fullname = item.ProfessorNome;
                response.Email = item.ProfessorEmail;
            }

            return response;
        }

        private CourseResponse CriarDisciplinaMoodle(Disciplina item, long moodleCategoryId)
        {
            CreateCourseClient client = CreateCourseClient;            

            BuildMoodleClient(client, MoodleTokenType.OfficialMoodleApiFunctions);

            CourseRequest request = new CourseRequest()
            {
                CategoryId = moodleCategoryId,
                Fullname = item.DisciplinaNome,
                Shortname = item.ShortName,
                Visible = 1
            };

            Task<CourseResponse> task = client.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
            CourseResponse response = task.Result;
            return response;
        }

        private CategoryResponse CriarCursoMoodle(Disciplina item)
        {
            CreateCategoryClient client = CreateCategoryClient;

            BuildMoodleClient(client, MoodleTokenType.OfficialMoodleApiFunctions);

            CategoryRequest request = new CategoryRequest()
            {
                Description = "Curso de " + item.CursoDescricao,
                DescriptionFormat = MoodleDescriptionFormat,
                Name = item.CursoDescricao,
                Parent = MoodleCategoryParent
            };

            Task<CategoryResponse> task = client.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
            CategoryResponse response = task.Result;
            return response;
        }

        private InternalMoodleData VerifyIfExistsOnMoodleAndCreateIfDont(Professor professor, Disciplina item)
        {
            //category
            long? moodleCategoryId = item.GetMoodleCursoId(ModalidadeAtual, GetCategoryByNameClient, HttpClient);

            if (!moodleCategoryId.HasValue)
            {
                CategoryResponse categoryResponse = CriarCursoMoodle(item);
                moodleCategoryId = categoryResponse?.Id;
            }

            if (moodleCategoryId.HasValue)
            {
                MoodleFromToCache.AddCategory(ModalidadeAtual.IdModalidade, item.CursoDescricao, moodleCategoryId.Value);                
            }
            else
            {
                throw new MoodleDataNotExistsException($"O curso [{item.CursoDescricao}] não está cadastrado no MOODLE.");
            }

            // course
            long? moodleCourseId = item.GetMoodleDisciplinaId(Configuration, ModalidadeAtual, GetCourseByNameClient, HttpClient);

            if (!moodleCourseId.HasValue)
            {
                CourseResponse courseResponse = CriarDisciplinaMoodle(item, moodleCategoryId.Value);
                moodleCourseId = courseResponse?.Id;
            }

            if (moodleCourseId.HasValue)
            {
                MoodleFromToCache.AddCourse(ModalidadeAtual.IdModalidade, item.DisciplinaNome, moodleCourseId.Value);
            }
            else
            {
                throw new MoodleDataNotExistsException($"A disciplina [{item.DisciplinaNome}] não está cadastrada no MOODLE.");
            }

            //user
            long? moodleUserId = professor.GetMoodleUserId(ModalidadeAtual, GetUserByUsernameClient, HttpClient);

            if (!moodleUserId.HasValue)
            {
                UserResponse userResponse = CriarUsuarioMoodle(professor);
                moodleUserId = userResponse?.Id;
            }

            if (moodleUserId.HasValue)
            {
                MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, professor.ProfessorCpf, moodleUserId.Value);
            }
            else
            {
                throw new MoodleDataNotExistsException($"O professor [{professor.ProfessorCpf} | {professor.ProfessorNome}] não está cadastrado no MOODLE.");
            }

            return new InternalMoodleData()
            {
                MoodleCategoryId = moodleCategoryId.Value,
                MoodleCourseId = moodleCourseId.Value,
                MoodleUserId = moodleUserId.Value
            };
        }

        private void Enrol(InternalMoodleData data)
        {
            EnrolmentClient client = EnrolmentClient;

            BuildMoodleClient(client, MoodleTokenType.OfficialMoodleApiFunctions);

            EnrolmentRequest request = new EnrolmentRequest()
            {
                CourseId = data.MoodleCourseId,
                UserId = data.MoodleUserId,
                RoleId = Constantes.ROLEID_PROFESSOR
            };

            Task<EmptyResponse> task = client.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
        }

        private GetEnrolmentsByUserIdResponse GetUserEnrolment(InternalMoodleData moodleData)
        {
            GetEnrolementsrByUserIdClient client = GetEnrolementsrByUserIdClient;

            BuildMoodleClient(client, MoodleTokenType.LocalMoodleExternalApiGetInfoToken);

            GetEnrolmentsByUserIdRequest request = new GetEnrolmentsByUserIdRequest()
            {
                UserId = moodleData.MoodleUserId,
                CategoryId = moodleData.MoodleCategoryId,
                CourseId = moodleData.MoodleCourseId,
                RoleId = Constantes.ROLEID_ALUNO
            };

            Task<GetEnrolmentsByUserIdResponse> task = client.Post(request);
            task.Wait();
                        
            GetEnrolmentsByUserIdResponse response = task.Result;
            return response;
        }

        private GetEnrolmentsByUserIdResponse SendDisciplina(InternalMoodleData moodleData)
        {
            Enrol(moodleData);
            GetEnrolmentsByUserIdResponse response = GetUserEnrolment(moodleData);
            return response;
        }


        protected void ProcessDisciplina(Professor professor, Disciplina disciplina, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            disciplina.DisciplinaNome = disciplina.GetNomeDisciplina(Configuration, ModalidadeAtual);
            disciplina.ShortName = disciplina.GetShortNameDisciplina(Configuration, ModalidadeAtual);

            try
            {
                InternalMoodleData moodleData = VerifyIfExistsOnMoodleAndCreateIfDont(professor, disciplina);
                GetEnrolmentsByUserIdResponse response = null;

                if (moodleData?.MoodleCategoryId > 0 && moodleData?.MoodleCourseId > 0 && moodleData?.MoodleUserId > 0)
                {
                    response = SendDisciplina(moodleData);
                }

                ProfessorDisciplinaImportedResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse> importedResult =
                    new ProfessorDisciplinaImportedResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse>()
                    {
                        Date = DateTime.Now,                        
                        Url = LastUrl,
                        Professor = professor,
                        Disciplina = disciplina,
                        Result = response
                    };

                Result.ImportedSuccessfully.Enqueue(importedResult);
                Log($"Disciplina [{disciplina.DisciplinaNome}] do professor [{professor.ProfessorCpf} | {professor.ProfessorNome}] adicionada.");
            }
            catch (MoodleDataNotExistsException mex)
            {
                var reason = new ProfessorDisciplinaNotImportedReason<ProfessorDisciplinaViewModel>()
                {                    
                    Url = LastUrl,
                    Professor = professor,
                    Disciplina = disciplina,
                    Reason = mex.Message
                };

                Result.NotImported.Enqueue(reason);
                Log(reason.Reason);
            }
            catch (AggregateException agex)
            {
                var exception = agex.InnerExceptions[0];

                var reason = new ProfessorDisciplinaNotImportedReason<ProfessorDisciplinaViewModel>()
                {                    
                    Url = LastUrl,
                    Professor = professor,
                    Disciplina = disciplina,
                    Exception = exception,
                    Reason = exception.Message
                };

                Result.NotImported.Enqueue(reason);
                Log(reason.Reason);
            }
            catch (Exception ex)
            {
                var reason = new ProfessorDisciplinaNotImportedReason<ProfessorDisciplinaViewModel>()
                {                    
                    Url = LastUrl,
                    Professor = professor,
                    Disciplina = disciplina,
                    Exception = ex,
                    Reason = ex.Message
                };

                Result.NotImported.Enqueue(reason);
                Log(reason.Reason);
            }
        }

        protected override void ProcessItem(ProfessorDisciplinaViewModel viewmodel, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            if (UseParallelism)
            {
                Parallel.ForEach(viewmodel.Disciplinas, (disciplina) =>
                {
                    ProcessDisciplina(viewmodel.Professor, disciplina, createClient, verifyClient);
                });
            }
            else
            {
                foreach (var disciplina in viewmodel.Disciplinas)
                {
                    ProcessDisciplina(viewmodel.Professor, disciplina, createClient, verifyClient);
                }
            }
        }

        protected override void ProcessWithParallelism(IEnumerable<ProfessorDisciplinaViewModel> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            data.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)                
                .ForAll((viewmodel) =>
                {
                    ProcessItem(viewmodel, createClient, verifyClient);
                });
        }

        protected override void ProcessWithRegularForeach(IEnumerable<ProfessorDisciplinaViewModel> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            /*var cpfs = new string[] {
                "638.819.421-49"
            };*/

            foreach (var viewmodel in data)
            {
                /*if (cpfs.Contains(viewmodel.Professor.ProfessorCpf))
                {
                    Debugger.Break();
                }*/

                ProcessItem(viewmodel, createClient, verifyClient);
            }
        }


        public override IEnumerable<ProfessorDisciplinaViewModel> GetData(string filter, bool active = true)
        {
            if(Professores?.Count() == 0)
            {
                Professores = ConsultaAcademicaService.GetProfessores(filter);
            }

            var data = new List<ProfessorDisciplinaViewModel>();

            foreach(var item in Professores)
            {
                ProfessorDisciplinaViewModel viewModel = null;

                if (item.Disciplinas?.Count() == 0)
                {
                    viewModel = new ProfessorDisciplinaViewModel()
                    {
                        Professor = item                        
                    };
                }
                else
                {
                    viewModel = new ProfessorDisciplinaViewModel()
                    {
                        Professor = item,
                        Disciplinas = item.Disciplinas
                    };
                }

                data.Add(viewModel);
            }
                        
            return data;
        }        

        public override ParallelSendResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse> SendAll()
        {
            Result = new ParallelSendResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse>();            

            IEnumerable<ProfessorDisciplinaViewModel> data = GetData("");

            if (data == null)
            {
                return Result;
            }

            var factory = new HttpClientFactory();

            HttpClient = factory.CreateMoodleHttpClient();
            CreateCourseClient = new CreateCourseClient();
            CreateCategoryClient = new CreateCategoryClient();
            CreateUserClient = new CreateUserClient();
            GetCategoryByNameClient = new GetCategoryByNameClient();
            GetCourseByNameClient = new GetCourseByNameClient();
            GetUserByUsernameClient = new GetUserByUsernameClient();
            EnrolmentClient = new EnrolmentClient();
            GetEnrolementsrByUserIdClient = new GetEnrolementsrByUserIdClient();

            CreateCourseClient.AddHttpClient(HttpClient);
            CreateCategoryClient.AddHttpClient(HttpClient);
            CreateUserClient.AddHttpClient(HttpClient);
            GetCategoryByNameClient.AddHttpClient(HttpClient);
            GetCourseByNameClient.AddHttpClient(HttpClient);
            GetUserByUsernameClient.AddHttpClient(HttpClient);
            EnrolmentClient.AddHttpClient(HttpClient);
            GetEnrolementsrByUserIdClient.AddHttpClient(HttpClient);

            foreach (var modalidade in Modalidades)
            {
                ModalidadeAtual = modalidade;

                this.AddMoodleBaseUrl(modalidade.MoodleUrl)
                    .AddMoodleToken(modalidade.MoodleToken)
                    .AddMoodleGetInfoServiceToken(modalidade.MoodleGetInfoServiceToken)
                    .AddMoodleServiceUrl(modalidade.MoodleServiceUrl);

                this.AddMoodleCategoryParent(modalidade.MoodleCategoryParent)
                    .AddMoodleDescriptionFormat(modalidade.MoodleDescriptionFormat);

                var professores = data.Where
                (
                    x => x.Professor
                          .Disciplinas
                          .Where(d => d.IdModalidade == modalidade.IdModalidade)
                          .Count() > 0
                )
                .ToArray();

                if (UseParallelism)
                {
                    ProcessWithParallelism(professores, null, null);                    
                }
                else
                {
                    ProcessWithRegularForeach(professores, null, null);
                }
            }

            return Result;
        }        
    }
}
