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
    public class AlunoDisciplinaProxy : BaseParallelConsultaAcademicaProxy<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse>
    {
        private IEnumerable<Aluno> Alunos;
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


        public AlunoDisciplinaProxy(IConsultaAcademicaService service) : base(service)
        {
            
        }


        public AlunoDisciplinaProxy AddAlunos(IEnumerable<Aluno> alunos)
        {
            Alunos = alunos;
            return this;
        }

        public AlunoDisciplinaProxy AddConfiguration(AppConfiguration configuration)
        {
            Configuration = configuration;
            return this;
        }

        public AlunoDisciplinaProxy AddMoodleCategoryParent(int parent)
        {
            MoodleCategoryParent = parent;
            return this;
        }

        public AlunoDisciplinaProxy AddMoodleDescriptionFormat(int format)
        {
            MoodleDescriptionFormat = format;
            return this;
        }


        private UserResponse CriarUsuarioMoodle(Aluno item)
        {
            CreateUserClient client = CreateUserClient;

            BuildMoodleClient(client, MoodleTokenType.OfficialMoodleApiFunctions);

            var nome = item.AlunoNomeSocial ?? item.AlunoNome;
            var tuple = nome.SepararNomeSobrenome();
            var matricula = item.AlunoMatricula.FormatarMatricula();

            if (item.AlunoEmail == null)
                item.AlunoEmail = "";

            UserRequest request = new UserRequest()
            {
                Email = item.AlunoEmail.TratarEmail(matricula),
                Username = item.AlunoCpf.DesformatarCpf(),
                Firstname = tuple.Item1,
                Lastname = tuple.Item2,
                Password = matricula
            };

            request.Preferences.Add("auth_forcepasswordchange", "1");

            Task<UserResponse> task = client.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
            UserResponse response = task.Result;

            if (response != null)
            {
                response.Fullname = nome;
                response.Email = request.Email;
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

        private InternalMoodleData VerifyIfExistsOnMoodleAndCreateIfDont(Aluno aluno, Disciplina item)
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
            long? moodleUserId = aluno.GetMoodleUserId(ModalidadeAtual, GetUserByUsernameClient, HttpClient);

            if (!moodleUserId.HasValue)
            {
                UserResponse userResponse = CriarUsuarioMoodle(aluno);
                moodleUserId = userResponse?.Id;
            }

            if (moodleUserId.HasValue)
            {
                MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, aluno.AlunoCpf, moodleUserId.Value);
            }
            else
            {
                throw new MoodleDataNotExistsException($"O aluno [{aluno.AlunoCpf} | {aluno.AlunoNomeSocial ?? aluno.AlunoNome}] não está cadastrado no MOODLE.");
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
                RoleId = Constantes.ROLEID_ALUNO
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


        protected void ProcessDisciplina(Aluno aluno, Disciplina disciplina, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            if (disciplina?.IdDisciplina == 0 || string.IsNullOrEmpty(disciplina?.DisciplinaNome))
            {
                return;
            }

            disciplina.DisciplinaNome = disciplina.GetNomeDisciplina(Configuration, ModalidadeAtual);
            disciplina.ShortName = disciplina.GetShortNameDisciplina(Configuration, ModalidadeAtual);

            try
            {
                InternalMoodleData moodleData = VerifyIfExistsOnMoodleAndCreateIfDont(aluno, disciplina);
                GetEnrolmentsByUserIdResponse response = null;

                if (moodleData?.MoodleCategoryId > 0 && moodleData?.MoodleCourseId > 0 && moodleData?.MoodleUserId > 0)
                {
                    response = SendDisciplina(moodleData);
                }

                AlunoDisciplinaImportedResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> importedResult =
                    new AlunoDisciplinaImportedResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse>()
                    {
                        Date = DateTime.Now,                        
                        Aluno = aluno,
                        Disciplina = disciplina,
                        Url = LastUrl,
                        Result = response
                    };

                Result.ImportedSuccessfully.Enqueue(importedResult);
                Log($"Disciplina {disciplina.DisciplinaNome} do aluno {aluno.AlunoNomeSocial ?? aluno.AlunoNome} adicionada.");
            }
            catch (MoodleDataNotExistsException mex)
            {
                var reason = new AlunoDisciplinaNotImportedReason<AlunoDisciplinaViewModel>()
                {                    
                    Url = LastUrl,
                    Aluno = aluno,
                    Disciplina = disciplina,
                    Reason = mex.Message
                };

                Result.NotImported.Enqueue(reason);
                Log($"Disciplina {disciplina.DisciplinaNome} não adicionada. {reason.Reason}");
            }
            catch (AggregateException agex)
            {
                var exception = agex.InnerExceptions[0];

                var reason = new AlunoDisciplinaNotImportedReason<AlunoDisciplinaViewModel>()
                {                 
                    Url = LastUrl,
                    Aluno = aluno,
                    Disciplina = disciplina,
                    Exception = exception,
                    Reason = exception.Message
                };

                Result.NotImported.Enqueue(reason);

                if (exception is MoodleResponseException moodleEx)
                {
                    Log($"Disciplina {disciplina.DisciplinaNome} não adicionada. {moodleEx.RawMoodleError}");
                }
                else
                {
                    Log($"Disciplina {disciplina.DisciplinaNome} não adicionada. {reason.Reason}");
                }
            }
            catch (Exception ex)
            {
                var reason = new AlunoDisciplinaNotImportedReason<AlunoDisciplinaViewModel>()
                {                    
                    Url = LastUrl,
                    Aluno = aluno,
                    Disciplina = disciplina,
                    Exception = ex,
                    Reason = ex.Message
                };

                Result.NotImported.Enqueue(reason);
                Log($"Disciplina {disciplina.DisciplinaNome} não adicionada. {reason.Reason}");
            }
        }

        protected override void ProcessItem(AlunoDisciplinaViewModel viewmodel, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {            
            viewmodel.Disciplinas = ConsultaAcademicaService.GetDisciplinasAluno(viewmodel.Aluno.IdAluno);

            if (viewmodel.Disciplinas?.Count() == 0)
            {
                Log($"Aluno {viewmodel.Aluno.AlunoNome} não tem disciplinas");
                return;
            }

            foreach (var disciplina in viewmodel.Disciplinas)
            {
                ProcessDisciplina(viewmodel.Aluno, disciplina, createClient, verifyClient);
            }
        }

        protected override void ProcessWithParallelism(IEnumerable<AlunoDisciplinaViewModel> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            data.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)                
                .ForAll((viewmodel) =>
                {
                    ProcessItem(viewmodel, createClient, verifyClient);
                });
        }

        protected override void ProcessWithRegularForeach(IEnumerable<AlunoDisciplinaViewModel> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            /*var cpfs = new string[] {
                "054.653.471-63",
                "041.699.381-86",
                "039.320.661-03",
                "039.281.621-01",
                "006.818.041-14",
                "038.654.121-30",
                "057.103.101-31"
            };*/

            foreach (var viewmodel in data)
            {
                /*if(cpfs.Contains(viewmodel.Aluno.AlunoCpf))
                {
                    Debugger.Break();
                }
                */
                ProcessItem(viewmodel, createClient, verifyClient);
            }
        }


        public override IEnumerable<AlunoDisciplinaViewModel> GetData(string filter, bool active = true)
        {
            if (Alunos?.Count() == 0)
            {
                Alunos = ConsultaAcademicaService.GetAlunosBySemestre(SemestreAtual, filter);
            }

            var data = new List<AlunoDisciplinaViewModel>();
            var alunosAtivos = Alunos.Where(x => x.AtivoAluno == true).ToArray();

            foreach (var item in alunosAtivos)
            {
                var viewModel = new AlunoDisciplinaViewModel()
                {
                    Aluno = item                    
                };

                data.Add(viewModel);
            }

            return data;
        }        

        public override ParallelSendResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> SendAll()
        {
            Result = new ParallelSendResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse>();            

            IEnumerable<AlunoDisciplinaViewModel> data = GetData("");

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

                var alunos = data.Where(x => x.Aluno.IdModalidade == modalidade.IdModalidade).ToArray();

                if (UseParallelism)
                {
                    ProcessWithParallelism(alunos, null, null);                    
                }
                else
                {
                    ProcessWithRegularForeach(alunos, null, null);
                }
            }

            return Result;
        }
    }
}
