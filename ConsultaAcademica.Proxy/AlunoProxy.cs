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
    public class AlunoProxy : BaseParallelConsultaAcademicaProxy<Aluno, UserResponse>
    {
        private IEnumerable<Aluno> Alunos;
        private volatile Modalidade ModalidadeAtual;


        public AlunoProxy(IConsultaAcademicaService service) : base(service)
        {
            
        }


        public AlunoProxy AddAlunos(IEnumerable<Aluno> alunos)
        {
            Alunos = alunos;
            return this;
        }
        

        protected override void ProcessItem(Aluno item, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            try
            {
                long? cachedMoodleId = MoodleFromToCache.GetCachedMoodleUser(ModalidadeAtual.IdModalidade, item.AlunoCpf);
                LastUrl = "cached_value";

                if (cachedMoodleId.HasValue)
                {
                    var reason = new NotImportedReason<Aluno>()
                    {
                        Data = item,
                        Url = LastUrl,
                        Reason = $"Aluno [{item.AlunoCpf} | {item.AlunoNomeSocial ?? item.AlunoNome}] já está adicionado ao MOODLE ({LastUrl})."
                    };

                    Result.NotImported.Enqueue(reason);
                    Log(reason.Reason);
                    return;
                }

                UserResponse exists = VerifyIfExists(verifyClient, item.AlunoCpf);

                if (exists?.Id > 0)
                {
                    MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, item.AlunoCpf, exists.Id);

                    var reason = new NotImportedReason<Aluno>()
                    {
                        Data = item,
                        Url = LastUrl,
                        Reason = $"Aluno [{item.AlunoCpf} | {item.AlunoNomeSocial ?? item.AlunoNome}] já está adicionado ao MOODLE ({LastUrl})."
                    };

                    Result.NotImported.Enqueue(reason);
                    Log(reason.Reason);
                    return;
                }
                
                if (item.AtivoAluno)
                {
                    UserResponse response = SendItem(createClient, item);

                    ImportedResult<Aluno, UserResponse> importedResult = new ImportedResult<Aluno, UserResponse>()
                    {
                        Date = DateTime.Now,
                        Data = item,
                        Url = LastUrl,
                        Result = response,
                        Active = true,
                    };

                    Result.ImportedSuccessfully.Enqueue(importedResult);
                    MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, item.AlunoCpf, response.Id);
                }
                else
                {
                    SuspendedUserResult suspendedUserResult = this.SuspendItem(item, ModalidadeAtual, createClient.GetUnderlyingHttpClient());

                    if (!suspendedUserResult.MoodleId.HasValue)
                    {
                        throw new MoodleDataNotExistsException($"Tentativa de suspender usuário falhou. O aluno [{item.AlunoCpf} | {item.AlunoNomeSocial ?? item.AlunoNome}] não está cadastrado no MOODLE");
                    }

                    var nome = item.AlunoNomeSocial ?? item.AlunoNome;                    
                    var matricula = item.AlunoMatricula.FormatarMatricula();

                    if (item.AlunoEmail == null)
                        item.AlunoEmail = "";

                    UserResponse response = new UserResponse()
                    {
                        Email = item.AlunoEmail.TratarEmail(item.AlunoMatricula),
                        Id = suspendedUserResult.MoodleId.Value,
                        Fullname = nome,
                        Username = item.AlunoCpf.DesformatarCpf()                        
                    };

                    ImportedResult<Aluno, UserResponse> importedResult = new ImportedResult<Aluno, UserResponse>()
                    {
                        Date = DateTime.Now,
                        Data = item,
                        Url = LastUrl,
                        Result = response,
                        Active = false
                    };

                    Result.ImportedSuccessfully.Enqueue(importedResult);
                    MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, item.AlunoCpf, suspendedUserResult.MoodleId.Value);
                }
                
                Log($"Aluno {item.AlunoNome} adicionado.");
            }
            catch (MoodleDataNotExistsException mex)
            {
                var reason = new NotImportedReason<Aluno>()
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

                var reason = new NotImportedReason<Aluno>()
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
                var reason = new NotImportedReason<Aluno>()
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

        protected override void ProcessWithParallelism(IEnumerable<Aluno> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            data.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)                            
                .ForAll((item) =>
                {
                    ProcessItem(item, createClient, verifyClient);
                });
        }

        protected override void ProcessWithRegularForeach(IEnumerable<Aluno> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            foreach (var item in data)
            {
                ProcessItem(item, createClient, verifyClient);
            }
        }


        public override IEnumerable<Aluno> GetData(string filter, bool active = true)
        {
            if(Alunos?.Count() > 0)
            {
                return Alunos;
            }

            Alunos = ConsultaAcademicaService.GetAlunosBySemestre(SemestreAtual, filter);
            return Alunos;
        }
      
        public override UserResponse VerifyIfExists(AbstractMoodleServiceClient client, string filter)
        {
            GetUserByUsernameClient verifyClient = (GetUserByUsernameClient)client;

            GetByUsernameRequest request = new GetByUsernameRequest()
            {
                Username = filter.DesformatarCpf()
            };

            Task<UserResponse> task = verifyClient.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
            UserResponse response = task.Result;
            return response;
        }

        public override UserResponse SendItem(AbstractMoodleServiceClient client, Aluno item)
        {
            CreateUserClient createClient = (CreateUserClient) client;
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

            Task<UserResponse> task = createClient.Post(request);
            task.Wait();

            UserResponse response = task.Result;
            LastUrl = createClient.LastUrl;

            if(response != null)
            {
                response.Fullname = item.AlunoNomeSocial;
                response.Email = item.AlunoEmail;
            }

            return response;
        }

        public override ParallelSendResult<Aluno, UserResponse> SendAll()
        {
            Result = new ParallelSendResult<Aluno, UserResponse>();
            System.GC.Collect();

            IEnumerable<Aluno> data = GetData("");

            if (data == null)
            {
                return Result;
            }

            var factory = new HttpClientFactory();

            using (var httpClient = factory.CreateMoodleHttpClient())
            {
                var createClient = new CreateUserClient();
                var verifyClient = new GetUserByUsernameClient();

                // Sharing the same HttpClient instance to improve performance
                verifyClient.AddHttpClient(httpClient);
                createClient.AddHttpClient(httpClient);

                foreach (var modalidade in Modalidades)
                {
                    ModalidadeAtual = modalidade;

                    this.AddMoodleBaseUrl(modalidade.MoodleUrl)
                        .AddMoodleToken(modalidade.MoodleToken)
                        .AddMoodleGetInfoServiceToken(modalidade.MoodleGetInfoServiceToken)
                        .AddMoodleServiceUrl(modalidade.MoodleServiceUrl);

                    BuildMoodleClient(createClient, MoodleTokenType.OfficialMoodleApiFunctions);
                    BuildMoodleClient(verifyClient, MoodleTokenType.LocalMoodleExternalApiGetInfoToken);

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
