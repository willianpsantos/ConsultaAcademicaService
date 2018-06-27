using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.MoodleClient.Response;
using ConsultaAcademica.Proxy;
using ConsultaAcademica.Service;
using ConsultaAcademica.Data;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ConsultaAcademica.Proxy.DataCache;

namespace ConsultaAcademica.Importador
{
    public class Resultados
    {
        public SendResult<Curso, CategoryResponse> CursosResult { get; set; }

        public ParallelSendResult<Disciplina, CourseResponse> DisciplinasResult { get; set; }

        public ParallelSendResult<Professor, UserResponse> ProfessoresResult { get; set; }

        public ParallelSendResult<Aluno, UserResponse> AlunosResult { get; set; }

        public ParallelSendResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse> DisciplinasProfessorResult { get; set; }

        public ParallelSendResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> DisciplinasAlunoResult { get; set; }
    } 

    public class ImportadorServiceController : IDisposable
    {
        protected AbstractServiceBuilder ServiceBuilder;        
        protected AppConfiguration Configuration;
        protected ConsultaAcademicaService ConsultaAcademicaService;
        protected SincroniaService SincroniaService;
        protected Modalidade[] Modalidades;
        
        public bool CanLog { get; set; }

        public bool UseParallelism { get; set; }


        public ImportadorServiceController(AbstractServiceBuilder builder, AppConfiguration configuration)
        {
            ServiceBuilder = builder;
            Configuration = configuration;
            ConsultaAcademicaService = ServiceBuilder.GetService<ConsultaAcademicaService>("ConsultaAcademicaService");
            SincroniaService = ServiceBuilder.GetService<SincroniaService>("SincroniaService");
            Modalidades = (Modalidade[])GetModalidades();

            CanLog = false;
            UseParallelism = true;
        }


        public ImportadorServiceController AddServiceBuilder(AbstractServiceBuilder builder)
        {
            ServiceBuilder = builder;
            return this;
        }

        public ImportadorServiceController AddConfiguration(AppConfiguration configuration)
        {
            Configuration = configuration;
            return this;
        }


        public void ConfigureMoodleCache()
        {            
            var cacheBuilder = new MoodleFromToCacheAdapter(ConsultaAcademicaService);            

            cacheBuilder.AddConfiguration(Configuration)
                        .AddModalidades(Modalidades)
                        .FillCategories()
                        .FillAlumns()
                        .FillTeachers()
                        .FillCourses();
        }

        public IEnumerable<Modalidade> GetModalidades()
        {
            IEnumerable<Modalidade> modalidades = ConsultaAcademicaService.GetModalidades("", Configuration);
            return modalidades;
        }

        public IEnumerable<Aluno> GetAlunos()
        {
            return ConsultaAcademicaService.GetAlunosBySemestre(Configuration.SemestreAtual);
        }

        public IEnumerable<Professor> GetProfessores()
        {
            var professores = ConsultaAcademicaService.GetProfessores();

            foreach (var item in professores)
            {
                item.Disciplinas = ConsultaAcademicaService.GetDisciplinasProfessor(Configuration.SemestreAtual, item.IdProfessor);
            }

            return professores;
        }

        public SendResult<Curso, CategoryResponse> ImportarCursos()
        {
            CursoProxy cursoProxy = new CursoProxy(ConsultaAcademicaService)
            {
                CanLog = CanLog
            };

            SendResult<Curso, CategoryResponse> result = cursoProxy.AddModalidades(Modalidades)
                                                                   .AddSemestreAtual(Configuration.SemestreAtual) 
                                                                   .SendAll();

            return result;
        }

        public ParallelSendResult<Professor, UserResponse> ImportarProfessores(IEnumerable<Professor> professores)
        {
            ProfessorProxy professorProxy = new ProfessorProxy(ConsultaAcademicaService)
            {
                UseParallelism = UseParallelism,
                CanLog = CanLog
            };

            ParallelSendResult<Professor, UserResponse> result = professorProxy.AddProfessores(professores)
                                                                               .AddModalidades(Modalidades)
                                                                               .AddSemestreAtual(Configuration.SemestreAtual) 
                                                                               .SendAll();

            return result;
        }

        public ParallelSendResult<Aluno, UserResponse> ImportarAlunos(IEnumerable<Aluno> alunos)
        {
            AlunoProxy alunoProxy = new AlunoProxy(ConsultaAcademicaService)
            {
                UseParallelism = UseParallelism,
                CanLog = CanLog
            };

            ParallelSendResult<Aluno, UserResponse> result = alunoProxy.AddAlunos(alunos)
                                                                       .AddModalidades(Modalidades)
                                                                       .AddSemestreAtual(Configuration.SemestreAtual)
                                                                       .SendAll();

            return result;
        }

        public ParallelSendResult<Disciplina, CourseResponse> ImportarDisciplinas()
        {
            DisciplinaProxy disciplinaProxy = new DisciplinaProxy(ConsultaAcademicaService)
            {
                UseParallelism = UseParallelism,
                CanLog = CanLog
            };

            ParallelSendResult<Disciplina, CourseResponse> result = disciplinaProxy
                                                                        .AddConfiguration(Configuration)
                                                                        .AddModalidades(Modalidades)
                                                                        .AddSemestreAtual(Configuration.SemestreAtual)
                                                                        .SendAll();

            return result;
        }

        public ParallelSendResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> ImportarAlunoDisciplinas(IEnumerable<Aluno> alunos)
        {
            AlunoDisciplinaProxy alunoProxy = new AlunoDisciplinaProxy(ConsultaAcademicaService)
            {
                UseParallelism = false,
                CanLog = CanLog
            };

            ParallelSendResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> result = 
                alunoProxy.AddAlunos(alunos)
                          .AddConfiguration(Configuration)
                          .AddModalidades(Modalidades)                          
                          .AddSemestreAtual(Configuration.SemestreAtual)                          
                          .SendAll();

            return result;
        }

        public ParallelSendResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse> ImportarProfessorDisciplinas(IEnumerable<Professor> professores)
        {
            ProfessorDisciplinaProxy professorProxy = new ProfessorDisciplinaProxy(ConsultaAcademicaService)
            {
                UseParallelism = UseParallelism,
                CanLog = CanLog
            };

            ParallelSendResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse> result =
                professorProxy.AddProfessores(professores)
                              .AddConfiguration(Configuration)
                              .AddModalidades(Modalidades)
                              .AddSemestreAtual(Configuration.SemestreAtual)
                              .SendAll();

            return result;
        }

        public Resultados Sincronizar()
        {
            SincroniaProxy sincroniaProxy = new SincroniaProxy(SincroniaService, ConsultaAcademicaService);
            IEnumerable<Aluno> alunos = sincroniaProxy.GetData();

            if (alunos?.Count() == 0)
            {
                return null;
            }

            AlunoProxy alunoProxy = new AlunoProxy(ConsultaAcademicaService)
            {
                UseParallelism = UseParallelism,
                CanLog = CanLog
            };

            ParallelSendResult<Aluno, UserResponse> alunoResult =
                alunoProxy.AddAlunos(alunos)
                          .AddModalidades(Modalidades)
                          .AddSemestreAtual(Configuration.SemestreAtual)                          
                          .SendAll();
            
            AlunoDisciplinaProxy alunoDisciplinaProxy = new AlunoDisciplinaProxy(ConsultaAcademicaService)
            {
                UseParallelism = UseParallelism,
                CanLog = CanLog
            };

            ParallelSendResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> disciplinaResult =
                alunoDisciplinaProxy.AddAlunos(alunos)
                                    .AddConfiguration(Configuration)
                                    .AddModalidades(Modalidades)
                                    .AddSemestreAtual(Configuration.SemestreAtual)
                                    .SendAll();

            SincroniaService.ResetarAlunosEAD();

            return new Resultados()
            {
                AlunosResult = alunoResult,
                DisciplinasAlunoResult = disciplinaResult
            };
        }

        public void SalvarLogs(Resultados resultados, bool sincronia = false)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                if (resultados == null)
                    return;

                var logRepository = new LogImportacaoRepository();

                var logProxy = new LogImportacaoProxy(logRepository)
                {
                    Sincronia = sincronia
                };

                if (resultados.CursosResult != null)
                    logProxy.SaveCoursesLogs(resultados.CursosResult);

                if (resultados.DisciplinasResult != null)
                    logProxy.SaveDisciplinesResult(resultados.DisciplinasResult);

                if (resultados.AlunosResult != null)
                    logProxy.SaveAlumnLogs(resultados.AlunosResult);

                if (resultados.ProfessoresResult != null)
                    logProxy.SaveProfessorLogs(resultados.ProfessoresResult);

                if (resultados.DisciplinasAlunoResult != null)
                    logProxy.SaveAlumnDisciplinesLogs(resultados.DisciplinasAlunoResult);

                if (resultados.DisciplinasProfessorResult != null)
                    logProxy.SaveProfessorDisciplinesLogs(resultados.DisciplinasProfessorResult);

                logRepository.Dispose();

                if (CanLog)
                    System.Console.WriteLine("Logs salvos");

                System.GC.Collect();
            });
        }

        public void Dispose()
        {
                      
        }
    }
}
