using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service.Contract;
using ConsultaAcademica.MoodleClient;
using ConsultaAcademica.MoodleClient.Request;
using ConsultaAcademica.MoodleClient.Response;

namespace ConsultaAcademica.Proxy
{
    public class CursoProxy : BaseConsultaAcademicaProxy<Curso, CategoryResponse>
    {
        private int MoodleCategoryParent;
        private int MoodleDescriptionFormat;        


        public CursoProxy(IConsultaAcademicaService service) : base(service)
        {
            
        }


        public override IEnumerable<Curso> GetData(string filter, bool active = true)
        {
            return ConsultaAcademicaService.GetCursosBySemestre(SemestreAtual, filter);
        }
        
        public CursoProxy AddMoodleCategoryParent(int parent)
        {
            MoodleCategoryParent = parent;
            return this;
        }

        public CursoProxy AddMoodleDescriptionFormat(int format)
        {
            MoodleDescriptionFormat = format;
            return this;
        }
                
        public override CategoryResponse VerifyIfExists(string filter)
        {
            GetCategoryByNameClient client = new GetCategoryByNameClient();

            BuildMoodleClient(client, MoodleTokenType.LocalMoodleExternalApiGetInfoToken);

            GetByNameRequest request = new GetByNameRequest()
            {
                Name = filter
            };

            Task<CategoryResponse> task = client.Post(request);
            task.Wait();

            LastUrl = client.LastUrl;
            CategoryResponse response = task.Result;
            return response;
        }

        public override CategoryResponse SendItem(Curso item)
        {
            CreateCategoryClient client = new CreateCategoryClient();

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

        public override SendResult<Curso, CategoryResponse> SendAll()
        {
            SendResult<Curso, CategoryResponse> result = new SendResult<Curso, CategoryResponse>();
            IEnumerable<Curso> data = GetData("");

            if (data == null)
            {
                return result;
            }
            
            foreach (var modalidade in Modalidades)
            {
                var cursos = data.Where(x => x.IdModalidade == modalidade.IdModalidade).ToArray();

                this.AddMoodleBaseUrl(modalidade.MoodleUrl)
                    .AddMoodleToken(modalidade.MoodleToken)
                    .AddMoodleGetInfoServiceToken(modalidade.MoodleGetInfoServiceToken)
                    .AddMoodleServiceUrl(modalidade.MoodleServiceUrl);

                this.AddMoodleCategoryParent(modalidade.MoodleCategoryParent)
                    .AddMoodleDescriptionFormat(modalidade.MoodleDescriptionFormat);

                foreach (var item in cursos)
                {
                    try
                    {
                        long? cachedMoodleId = MoodleFromToCache.GetCachedMoodleCategory(modalidade.IdModalidade, item.CursoDescricao);

                        if (cachedMoodleId.HasValue)
                        {
                            LastUrl = "cached_value";

                            var reason = new NotImportedReason<Curso>()
                            {
                                Data = item,
                                Url = LastUrl,
                                Reason = $"Curso [{item.CursoDescricao}] já está adicionado ao MOODLE ({LastUrl})."
                            };

                            result.NotImported.Enqueue(reason);
                            Log(reason.Reason);
                            continue;
                        }

                        CategoryResponse exists = VerifyIfExists(item.CursoDescricao);

                        if (exists?.Id > 0)
                        {
                            MoodleFromToCache.AddCategory(modalidade.IdModalidade, item.CursoDescricao, exists.Id);

                            var reason = new NotImportedReason<Curso>()
                            {
                                Data = item,
                                Url = LastUrl,
                                Reason = $"Curso [{item.CursoDescricao}] já está adicionado ao MOODLE ({LastUrl})."
                            };

                            result.NotImported.Enqueue(reason);
                            Log(reason.Reason);
                            continue;
                        }

                        CategoryResponse response = SendItem(item);

                        ImportedResult<Curso, CategoryResponse> importedResult = new ImportedResult<Curso, CategoryResponse>()
                        {
                            Date = DateTime.Now,
                            Data = item,
                            Url = LastUrl,
                            Result = response
                        };

                        result.ImportedSuccessfully.Enqueue(importedResult);
                        Log($"Curso [{item.CursoDescricao}] adicionado");
                    }
                    catch (Exception ex)
                    {
                        var reason = new NotImportedReason<Curso>()
                        {
                            Data = item,
                            Url = LastUrl,
                            Exception = ex
                        };

                        result.NotImported.Enqueue(reason);
                        Log(reason.Reason);
                    }
                }
            }

            return result;
        }
    }
}
