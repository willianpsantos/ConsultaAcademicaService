using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service.Contract;
using ConsultaAcademica.MoodleClient;
using ConsultaAcademica.MoodleClient.Response;
using ConsultaAcademica.MoodleClient.Request;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using ConsultaAcademica.Proxy.Result;

namespace ConsultaAcademica.Proxy
{
    public class DisciplinaProxy : BaseParallelConsultaAcademicaProxy<Disciplina, CourseResponse>
    {
        private IEnumerable<Disciplina> Disciplinas;        
        private GetCategoryByNameClient GetCategoryByNameClient;
        private AppConfiguration Configuration;

        private volatile Modalidade ModalidadeAtual;


        public DisciplinaProxy(IConsultaAcademicaService service) : base(service)
        {
            
        }


        public DisciplinaProxy AddDisciplinas(IEnumerable<Disciplina> disciplinas)
        {
            Disciplinas = disciplinas;
            return this;
        }

        public DisciplinaProxy AddConfiguration(AppConfiguration configuration)
        {
            Configuration = configuration;
            return this;
        }
        

        protected override void ProcessItem(Disciplina item, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            try
            {
                item.DisciplinaNome = item.GetNomeDisciplina(Configuration, ModalidadeAtual);
                item.ShortName = item.GetShortNameDisciplina(Configuration, ModalidadeAtual);

                long? cachedMoodleId = MoodleFromToCache.GetCachedMoodleCourse(ModalidadeAtual.IdModalidade, item.DisciplinaNome);

                if(cachedMoodleId.HasValue)
                {
                    LastUrl = "cached_value";

                    var reason = new NotImportedReason<Disciplina>()
                    {
                        Data = item,
                        Url = LastUrl,
                        Reason = $"Disciplina [{item.DisciplinaNome}] já está adicionada ao MOODLE ({LastUrl})."
                    };

                    Result.NotImported.Enqueue(reason);
                    Log(reason.Reason);
                    return;
                }

                CourseResponse exists = VerifyIfExists(verifyClient, item.DisciplinaNome);

                if (exists?.Id > 0)
                {
                    MoodleFromToCache.AddCourse(ModalidadeAtual.IdModalidade, item.DisciplinaNome, exists.Id);

                    var reason = new NotImportedReason<Disciplina>()
                    {
                        Data = item,
                        Url = LastUrl,
                        Reason = $"Disciplina [{item.DisciplinaNome}] já está adicionada ao MOODLE ({LastUrl})."
                    };

                    Result.NotImported.Enqueue(reason);
                    Log(reason.Reason);
                    return;
                }

                CourseResponse response = SendItem(createClient, item);

                ImportedResult<Disciplina, CourseResponse> importedResult = new ImportedResult<Disciplina, CourseResponse>()
                {
                    Date = DateTime.Now,
                    Data = item,
                    Url = LastUrl,
                    Result = response,
                    Active = true,
                };

                Result.ImportedSuccessfully.Enqueue(importedResult);
                MoodleFromToCache.AddCourse(ModalidadeAtual.IdModalidade, item.DisciplinaNome, response.Id);

                Log($"Disciplina {item.DisciplinaNome} adicionado.");
            }
            catch (MoodleDataNotExistsException mex)
            {
                var reason = new NotImportedReason<Disciplina>()
                {
                    Data = item,
                    Url = LastUrl,
                    Reason = mex.Message
                };

                Result.NotImported.Enqueue(reason);
                Log(reason.Reason);
            }
            catch (AggregateException agex)
            {
                var exception = agex.InnerExceptions[0];

                var reason = new NotImportedReason<Disciplina>()
                {
                    Data = item,
                    Url = LastUrl,
                    Reason = exception.Message,
                    Exception = exception
                };

                Result.NotImported.Enqueue(reason);
                Log(reason.Reason);
            }
            catch (Exception ex)
            {
                var reason = new NotImportedReason<Disciplina>()
                {
                    Data = item,
                    Url = LastUrl,
                    Exception = ex,
                    Reason = ex.Message
                };

                Result.NotImported.Enqueue(reason);
                Log(reason.Reason);
            }
        }

        protected override void ProcessWithParallelism(IEnumerable<Disciplina> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            data.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)                            
                .ForAll((item) =>
                {
                    ProcessItem(item, createClient, verifyClient);
                });
        }

        protected override void ProcessWithRegularForeach(IEnumerable<Disciplina> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            foreach (var item in data)
            {
                ProcessItem(item, createClient, verifyClient);
            }
        }


        public override IEnumerable<Disciplina> GetData(string filter, bool active = true)
        {
            if(Disciplinas?.Count() > 0)
            {
                return Disciplinas;
            }

            Disciplinas = ConsultaAcademicaService.GetDisciplinas(SemestreAtual, filter);
            return Disciplinas;
        }
      
        public override CourseResponse VerifyIfExists(AbstractMoodleServiceClient client, string filter)
        {
            GetCourseByNameClient verifyClient = (GetCourseByNameClient)client;

            GetByNameRequest request = new GetByNameRequest()
            {
                Name = filter
            };

            Task<CourseResponse> task = verifyClient.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
            CourseResponse response = task.Result;
            return response;
        }

        public override CourseResponse SendItem(AbstractMoodleServiceClient client, Disciplina item)
        {
            CreateCourseClient createClient = (CreateCourseClient) client;
            long? categoryId = item.GetMoodleCursoId(ModalidadeAtual, GetCategoryByNameClient, client.GetUnderlyingHttpClient());

            if(!categoryId.HasValue)
            {
                throw new MoodleDataNotExistsException($"O curso [{item.CursoDescricao}]´nõa está adicionado ao MOODLE.");
            }
            
            CourseRequest request = new CourseRequest()
            {
               CategoryId = categoryId.Value,
               Fullname = item.DisciplinaNome,
               Shortname = item.ShortName,
               Visible = 1
            };

            Task<CourseResponse> task = createClient.Post(request);
            task.Wait();

            CourseResponse response = task.Result;
            LastUrl = createClient.LastUrl;
            
            return response;
        }

        public override ParallelSendResult<Disciplina, CourseResponse> SendAll()
        {
            Result = new ParallelSendResult<Disciplina, CourseResponse>();
            System.GC.Collect();
            
            IEnumerable<Disciplina> data = GetData("");

            if (data == null)
            {
                return Result;
            }

            var factory = new HttpClientFactory();

            using (var httpClient = factory.CreateMoodleHttpClient())
            {
                GetCategoryByNameClient = new GetCategoryByNameClient();
                var createClient = new CreateCourseClient();
                var verifyClient = new GetCourseByNameClient();

                // Sharing the same HttpClient instance to improve performance
                verifyClient.AddHttpClient(httpClient);
                createClient.AddHttpClient(httpClient);
                GetCategoryByNameClient.AddHttpClient(httpClient);

                foreach (var modalidade in Modalidades)
                {
                    ModalidadeAtual = modalidade;

                    this.AddMoodleBaseUrl(modalidade.MoodleUrl)
                        .AddMoodleToken(modalidade.MoodleToken)
                        .AddMoodleGetInfoServiceToken(modalidade.MoodleGetInfoServiceToken)
                        .AddMoodleServiceUrl(modalidade.MoodleServiceUrl);

                    BuildMoodleClient(createClient, MoodleTokenType.OfficialMoodleApiFunctions);
                    BuildMoodleClient(verifyClient, MoodleTokenType.LocalMoodleExternalApiGetInfoToken);
                    BuildMoodleClient(GetCategoryByNameClient, MoodleTokenType.LocalMoodleExternalApiGetInfoToken);

                    var filtered = data.Where(x => x.IdModalidade == modalidade.IdModalidade).ToArray();
                    
                    if(UseParallelism)
                    {
                        ProcessWithParallelism(filtered, createClient, verifyClient);                        
                    }
                    else
                    {
                        ProcessWithRegularForeach(filtered, createClient, verifyClient);
                    }
                }
            }

            return Result;
        }
    }
}
