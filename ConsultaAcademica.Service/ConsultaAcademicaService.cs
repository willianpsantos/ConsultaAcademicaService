using System;
using System.Collections.Generic;
using ConsultaAcademica.Model;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ConsultaAcademica.Service.Contract;
using ConsultaAcademica.Core;

namespace ConsultaAcademica.Service
{
    public class ConsultaAcademicaService : AbstractService, IConsultaAcademicaService
    {
        private readonly string _ApiServiceName = "ConsultaAcademica";        

        public ConsultaAcademicaService() : base()
        {
            ServiceName = "ConsultaAcademicaService";
        }

        public ConsultaAcademicaService(string baseUrl) : base(baseUrl)
        {
            ServiceName = "ConsultaAcademicaService";
        }

        public Aluno GetAlunoById(long id)
        {
            Task<string> task = Get($"{_ApiServiceName}/GetAlunoById", new Dictionary<string, string>() { { "id", id.ToString() }  });
            task.Wait();

            string content = task.Result;

            if(string.IsNullOrEmpty(content))
            {
                return null;
            }

            Aluno aluno = JsonConvert.DeserializeObject<Aluno>(content);

            return aluno;
        }

        public IEnumerable<Disciplina> GetDisciplinasAluno(long id)
        {
            Task<string> task = Get($"{_ApiServiceName}/GetDisciplinasAluno", new Dictionary<string, string>() { { "id", id.ToString() } });
            task.Wait();

            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Disciplina[] { };
            }

            Disciplina[] disciplinas = JsonConvert.DeserializeObject<Disciplina[]>(content);

            return disciplinas;
        }

        public IEnumerable<Aluno> GetAlunos(string nomeMatriculaCpf = "")
        {
            Task<string> task = Get($"{_ApiServiceName}/GetAlunos", new Dictionary<string, string>() { { "nomeMatriculaCpf", nomeMatriculaCpf } });
            task.Wait();

            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Aluno[] { };
            }

            Aluno[] alunos = JsonConvert.DeserializeObject<Aluno[]>(content);

            return alunos;
        }

        public IEnumerable<Aluno> GetAlunosBySemestre(string periodoLetivo, string nomeMatriculaCpf = "")
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetAlunosBySemestre",
                new Dictionary<string, string>()
                {
                    { "periodoLetivo", periodoLetivo },
                    { "nomeMatriculaCpf", nomeMatriculaCpf }
                }
            );

            task.Wait();

            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Aluno[] { };
            }

            Aluno[] alunos = JsonConvert.DeserializeObject<Aluno[]>(content);

            return alunos;
        }

        public AreaConhecimento GetAreaConhecimentoById(long id)
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetAreaConhecimentoById",
                new Dictionary<string, string>()
                {
                    { "id", id.ToString() }                    
                }
            );

            task.Wait();

            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            AreaConhecimento areaConhecimento = JsonConvert.DeserializeObject<AreaConhecimento>(content);

            return areaConhecimento;
        }

        public IEnumerable<AreaConhecimento> GetAreasConhecimento(string nome = "")
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetAreasConhecimento",
                new Dictionary<string, string>()
                {
                    { "nome", nome }
                }
            );

            task.Wait();

            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new AreaConhecimento[] { };
            }

            AreaConhecimento[] areaConhecimento = JsonConvert.DeserializeObject<AreaConhecimento[]>(content);

            return areaConhecimento;
        }

        public Coordenador GetCoordenadorById(long id)
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetCoordenadorById",
                new Dictionary<string, string>()
                {
                    { "id", id.ToString() }
                }
            );

            task.Wait();

            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            Coordenador coordenador = JsonConvert.DeserializeObject<Coordenador>(content);

            return coordenador;
        }

        public IEnumerable<Coordenador> GetCoordenadores(string nomeMatriculaCpf = "", bool ativo = true)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetCoordenadores",
               new Dictionary<string, string>()
               {
                    { "nomeMatriculaCpf", nomeMatriculaCpf },
                    { "ativo", ativo ? "true" : "false" }
               }
           );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Coordenador[] { };
            }

            Coordenador[] coordenador = JsonConvert.DeserializeObject<Coordenador[]>(content);
            return coordenador;
        }

        public Curso GetCursoById(long id)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetCursoById",
               new Dictionary<string, string>()
               {
                    { "id", id.ToString() }
               }
           );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            Curso curso = JsonConvert.DeserializeObject<Curso>(content);
            return curso;
        }

        public IEnumerable<Curso> GetCursosBySemestre(string periodoLetivo, string nome = "")
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetCursosBySemestre",
               new Dictionary<string, string>()
               {
                    { "periodoLetivo", periodoLetivo },
                    { "nome", nome }
               }
           );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Curso[] { };
            }

            Curso[] curso = JsonConvert.DeserializeObject<Curso[]>(content);
            return curso;
        }
            
        public Diretor GetDiretorById(long id)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetDiretorById",
               new Dictionary<string, string>()
               {
                    { "id", id.ToString() }
               }
           );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            Diretor curso = JsonConvert.DeserializeObject<Diretor>(content);
            return curso;
        }

        public IEnumerable<Diretor> GetDiretores(string nomeMatriculaCpf = "", bool ativo = true)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetDiretores",
               new Dictionary<string, string>()
               {
                   { "nomeMatriculaCpf", nomeMatriculaCpf },
                   { "ativo", ativo.ToString() }
               }
           );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Diretor[] { };
            }

            Diretor[] diretores = JsonConvert.DeserializeObject<Diretor[]>(content);
            return diretores;
        }

        protected void ConfigurarModalidade(Modalidade modalidade, AppConfiguration configuration)
        {
            if (modalidade.IdModalidade == configuration.IdModalidadeApoioPresencial)
            {
                modalidade.MoodleUrl                 = configuration.MoodleApoioPresencialUrl;
                modalidade.MoodleServiceUrl          = configuration.MoodleApoioPresencialServiceUrl;
                modalidade.MoodleToken               = configuration.MoodleApoioPresencialToken;
                modalidade.MoodleGetInfoServiceToken = configuration.MoodleApoioPresencialGetInfoServiceToken;
                modalidade.MoodleCategoryParent      = configuration.IdCategoriaPadraoApoioPresencial;
                modalidade.MoodleDescriptionFormat   = configuration.DescriptionFormatApoioPresencial;
            }
            else if (modalidade.IdModalidade == configuration.IdModalidadeSemiPresencial)
            {
                modalidade.MoodleUrl                 = configuration.MoodleSemiPresencialUrl;
                modalidade.MoodleServiceUrl          = configuration.MoodleSemiPresencialServiceUrl;
                modalidade.MoodleToken               = configuration.MoodleSemiPresencialToken;
                modalidade.MoodleGetInfoServiceToken = configuration.MoodleSemiPresencialGetInfoServiceToken;
                modalidade.MoodleCategoryParent      = configuration.IdCategoriaPadraoSemiPresencial;
                modalidade.MoodleDescriptionFormat   = configuration.DescriptionFormatEad;
            }
            else if (modalidade.IdModalidade == configuration.IdModalidadeEad)
            {
                modalidade.MoodleUrl                 = configuration.MoodleEadUrl;
                modalidade.MoodleServiceUrl          = configuration.MoodleEadServiceUrl;
                modalidade.MoodleToken               = configuration.MoodleEadToken;
                modalidade.MoodleGetInfoServiceToken = configuration.MoodleEadGetInfoServiceToken;
                modalidade.MoodleCategoryParent      = configuration.IdCategoriaPadraoEad;
                modalidade.MoodleDescriptionFormat   = configuration.DescriptionFormatEad;
            }
        }

        protected IEnumerable<Modalidade> InternalGetModalidades(string nome, AppConfiguration configuration)
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetModalidades",
                new Dictionary<string, string>()
                {
                    { "nome", nome }
                }
            );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Modalidade[] { };
            }

            Modalidade[] modalidades = JsonConvert.DeserializeObject<Modalidade[]>(content);

            if (configuration == null)
            {
                return modalidades;
            }

            foreach (var item in modalidades)
            {
                ConfigurarModalidade(item, configuration);
            }

            return modalidades;
        }

        protected Modalidade InternalGetModalidadeById(long id, AppConfiguration configuration)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetModalidadeById",
               new Dictionary<string, string>()
               {
                   { "id", id.ToString() }
               }
            );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            Modalidade modalidade = JsonConvert.DeserializeObject<Modalidade>(content);

            if (configuration == null)
            {
                return modalidade;
            }

            ConfigurarModalidade(modalidade, configuration);
            return modalidade;
        }

        public Modalidade GetModalidadeById(long id)
        {
            return InternalGetModalidadeById(id, null);
        }

        public Modalidade GetModalidadeById(long id, AppConfiguration configuration)
        {
            return InternalGetModalidadeById(id, configuration);
        }

        public IEnumerable<Modalidade> GetModalidades(string nome = "")
        {
            return InternalGetModalidades(nome, null);
        }

        public IEnumerable<Modalidade> GetModalidades(string nome, AppConfiguration configuration)
        {
            return InternalGetModalidades(nome, configuration);
        }

        public Professor GetProfessorById(long id)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetProfessorById",
               new Dictionary<string, string>()
               {
                   { "id", id.ToString() }
               }
           );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            Professor professores = JsonConvert.DeserializeObject<Professor>(content);
            return professores;
        }

        public IEnumerable<Disciplina> GetDisciplinasProfessor(string periodoLetivo, long id)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetDisciplinasProfessor",
               new Dictionary<string, string>()
               {
                   {  "periodoLetivo", periodoLetivo },
                   { "id", id.ToString() }
               }
           );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Disciplina[] { };
            }

            Disciplina[] disciplinas = JsonConvert.DeserializeObject<Disciplina[]>(content);
            return disciplinas;
        }

        public IEnumerable<Professor> GetProfessores(string nomeMatriculaCpf = "")
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetProfessores", 
                new Dictionary<string, string>()
                {
                    { "nomeMatriculaCpf", nomeMatriculaCpf }
                }
            );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Professor[] { };
            }

            Professor[] professores = JsonConvert.DeserializeObject<Professor[]>(content);
            return professores;
        }

        public Turma GetTurmaById(long id)
        {
            Task<string> task = Get(
               $"{_ApiServiceName}/GetTurmaById",
               new Dictionary<string, string>()
               {
                   { "id", id.ToString() }
               }
           );

            task.Wait();

            string content = task.Result;
            Turma turma = JsonConvert.DeserializeObject<Turma>(content);

            return turma;
        }

        public IEnumerable<Turma> GetTurmasBySemestre(string periodoLetivo, string nomeSigla = "", bool ativo = true)
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetTurmasBySemestre",
                new Dictionary<string, string>()
                {
                    { "periodoLetivo", periodoLetivo },
                    { "nomeSigla", nomeSigla },
                    { "ativo", ativo.ToString() }
                }
            );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Turma[] { };
            }

            Turma[] turmas = JsonConvert.DeserializeObject<Turma[]>(content);
            return turmas;
        }

        public IEnumerable<Disciplina> GetDisciplinas(string periodoLetivo, string nomeDisciplina = "", string nomeCurso = "", string siglaCurso = "")
        {
            Task<string> task = Get(
                $"{_ApiServiceName}/GetDisciplinas",
                new Dictionary<string, string>()
                {
                    { "periodoLetivo", periodoLetivo },
                    { "nomeDisciplina", nomeDisciplina },
                    { "nomeCurso", nomeCurso },
                    { "siglaCurso", siglaCurso }
                }
            );

            task.Wait();
            string content = task.Result;

            if (string.IsNullOrEmpty(content))
            {
                return new Disciplina[] { };
            }

            Disciplina[] disciplinas = JsonConvert.DeserializeObject<Disciplina[]>(content);
            return disciplinas;
        }
    }
}
