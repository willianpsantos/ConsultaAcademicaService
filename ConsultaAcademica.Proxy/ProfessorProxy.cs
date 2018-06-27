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
    public class ProfessorProxy : BaseParallelConsultaAcademicaProxy<Professor, UserResponse>
    {
        private volatile Modalidade ModalidadeAtual;
        private IEnumerable<Professor> Professores;


        public ProfessorProxy(IConsultaAcademicaService service) : base(service)
        {
            
        }

        
        public ProfessorProxy AddProfessores(IEnumerable<Professor> professores)
        {
            Professores = professores;
            return this;
        }

        protected override void ProcessItem(Professor item, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            try
            {
                long? cachedMoodleId = MoodleFromToCache.GetCachedMoodleUser(ModalidadeAtual.IdModalidade, item.ProfessorCpf);

                if (cachedMoodleId.HasValue)
                {
                    LastUrl = "cached_value";

                    var reason = new NotImportedReason<Professor>()
                    {
                        Data = item,
                        Url = LastUrl,
                        Reason = $"Professor [{item.ProfessorCpf} | {item.ProfessorNome}] já está adicionado ao MOODLE ({LastUrl})."
                    };

                    Result.NotImported.Enqueue(reason);
                    Log(reason.Reason);
                    return;
                }

                UserResponse exists = VerifyIfExists(verifyClient, item.ProfessorCpf);

                if (exists?.Id > 0)
                {
                    MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, item.ProfessorCpf, exists.Id);

                    var reason = new NotImportedReason<Professor>()
                    {
                        Data = item,
                        Url = LastUrl,
                        Reason = $"Professor [{item.ProfessorCpf} | {item.ProfessorNome}] já está adicionado ao MOODLE ({LastUrl})."
                    };

                    Result.NotImported.Enqueue(reason);
                    Log(reason.Reason);
                    return;
                }

                if (item.AtivoProfessor)
                {
                    UserResponse response = SendItem(createClient, item);

                    ImportedResult<Professor, UserResponse> importedResult = new ImportedResult<Professor, UserResponse>()
                    {
                        Date = DateTime.Now,
                        Data = item,
                        Url = LastUrl,
                        Result = response,
                        Active = true,
                    };

                    Result.ImportedSuccessfully.Enqueue(importedResult);
                    Log($"Professor [{item.ProfessorCpf} | {item.ProfessorNome}] adicionado.");
                    MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, item.ProfessorCpf, response.Id);
                }
                else
                {
                    SuspendedUserResult suspendedUserResult = this.SuspendItem(item, ModalidadeAtual, createClient.GetUnderlyingHttpClient());

                    if (!suspendedUserResult.MoodleId.HasValue)
                    {
                        throw new MoodleDataNotExistsException($"Tentativa de suspender usuário falhou. O professor [{item.ProfessorCpf} | {item.ProfessorNome}] não está cadastrado no MOODLE");
                    }

                    var nome = item.ProfessorNome;
                    var matricula = item.ProfessorMatricula.FormatarMatricula();

                    if (item.ProfessorEmail == null)
                        item.ProfessorEmail = "";

                    UserResponse response = new UserResponse()
                    {
                        Email = item.ProfessorEmail.TratarEmail(item.ProfessorMatricula),
                        Id = suspendedUserResult.MoodleId.Value,
                        Fullname = nome,
                        Username = item.ProfessorCpf.DesformatarCpf()
                    };

                    ImportedResult<Professor, UserResponse> importedResult = new ImportedResult<Professor, UserResponse>()
                    {
                        Date = DateTime.Now,
                        Data = item,
                        Url = suspendedUserResult.LastUrl,
                        Result = response,
                        Active = false
                    };

                    Result.ImportedSuccessfully.Enqueue(importedResult);
                    Log($"Professor [{item.ProfessorCpf} | {item.ProfessorNome}] SUSPENSO.");
                    MoodleFromToCache.AddUser(ModalidadeAtual.IdModalidade, item.ProfessorCpf, suspendedUserResult.MoodleId.Value);
                }
            }
            catch (MoodleDataNotExistsException mex)
            {
                var reason = new NotImportedReason<Professor>()
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

                var reason = new NotImportedReason<Professor>()
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
                var reason = new NotImportedReason<Professor>()
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

        protected override void ProcessWithParallelism(IEnumerable<Professor> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            data.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .ForAll((item) =>
                {
                    ProcessItem(item, createClient, verifyClient);
                });
        }

        protected override void ProcessWithRegularForeach(IEnumerable<Professor> data, AbstractMoodleServiceClient createClient, AbstractMoodleServiceClient verifyClient)
        {
            foreach (var item in data)
            {
                ProcessItem(item, createClient, verifyClient);
            }
        }


        public override IEnumerable<Professor> GetData(string filter, bool active = true)
        {
            if(Professores?.Count() > 0)
            {
                return Professores;
            }

            var professores = ConsultaAcademicaService.GetProfessores(filter);

            foreach (var item in professores)
            {
                item.Disciplinas = ConsultaAcademicaService.GetDisciplinasProfessor(SemestreAtual, item.IdProfessor);
            }

            return professores;
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

        public override UserResponse SendItem(AbstractMoodleServiceClient client, Professor item)
        {
            CreateUserClient createCliente = (CreateUserClient)client;
            var tuple = item.ProfessorNome.SepararNomeSobrenome();
            var matricula = item.ProfessorMatricula.FormatarMatricula();

            if (item.ProfessorEmail == null)
                item.ProfessorEmail = "";

            UserRequest request = new UserRequest()
            {
                Email = item.ProfessorEmail.TratarEmail(matricula),
                Username = item.ProfessorCpf.DesformatarCpf(),
                Firstname = tuple.Item1,
                Lastname = tuple.Item2,
                Password = matricula
            };

            request.Preferences.Add("auth_forcepasswordchange", "1");

            Task<UserResponse> task = createCliente.Post(request);
            task.Wait();

            LastUrl = createCliente.LastUrl;
            UserResponse response = task.Result;

            if (response != null)
            {
                response.Fullname = item.ProfessorNome;
                response.Email = item.ProfessorEmail;
            }

            return response;
        }

        public override ParallelSendResult<Professor, UserResponse> SendAll()
        {
            Result = new ParallelSendResult<Professor, UserResponse>();
            System.GC.Collect();

            IEnumerable<Professor> data = GetData("");

            if (data == null)
            {
                return Result;
            }

            var factory = new HttpClientFactory();

            using (var httpClient = factory.CreateMoodleHttpClient())
            {
                CreateUserClient createClient = new CreateUserClient();
                GetUserByUsernameClient verifyClient = new GetUserByUsernameClient();

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

                    var professores = data.Where
                    (
                        x => 
                        (
                            x.Disciplinas
                             .Where(d => d.IdModalidade == modalidade.IdModalidade)
                             .Count() > 0
                        )
                    )
                    .ToArray();

                    if (UseParallelism)
                    {
                        ProcessWithParallelism(professores, createClient, verifyClient);                        
                    }
                    else
                    {
                        ProcessWithRegularForeach(professores, createClient, verifyClient);
                    }
                }
            }

            return Result;
        }

    }
}
