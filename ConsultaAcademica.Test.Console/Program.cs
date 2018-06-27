using ConsultaAcademica.Core;
using ConsultaAcademica.Importador;
using ConsultaAcademica.Service;
using ConsultaAcademica.Data;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ConsultaAcademica.Data.Interfaces;
using ConsultaAcademica.Proxy;
using ConsultaAcademica.Proxy.DataCache;
using ConsultaAcademica.Model;
using System.Diagnostics;

namespace ConsultaAcademica.Test.Console
{
    class Program
    {
        private static Timer _TimerSincronia;
        private static Timer _TimerImportacao;
        private static AppConfiguration _AppConfiguration;
        private static AbstractServiceBuilder _ServiceBuilder;
        private static bool SincroniaIniciada = false;
        private static bool ImportacaoIniciada = false;
                
        static void ImportarTest()
        {
            System.Console.WriteLine("Iniciando processo de importação...");
            System.Console.WriteLine("Criando o objeto ServiceBuilder...");

            _ServiceBuilder.AddAuthenticationService(new AutenticacaoService())
                           .AddService(new SincroniaService())
                           .AddService(new ConsultaAcademicaService())
                           .Build();

            System.Console.WriteLine("ServiceBuilder criado.");
            System.Console.WriteLine("Criando o objeto ImportadorServiceController...");

            ImportadorServiceController controller = new ImportadorServiceController(_ServiceBuilder, _AppConfiguration)
            {
                UseParallelism = false,
                CanLog = true
            };

            System.Console.WriteLine("ImportadorServiceController criado.");
            System.Console.WriteLine("Configurando o moodle cache...");

            controller.ConfigureMoodleCache();

            System.Console.WriteLine("Moodle cache configurado.");
            System.Console.WriteLine("Iniciando importação dos cursos...");

            var cursosResult = controller.ImportarCursos();

            System.Console.WriteLine("Cursos importados");
            System.Console.WriteLine("Iniciando importação das disciplinas...");

            var disciplinasResult = controller.ImportarDisciplinas();

            System.Console.WriteLine("Disciplinas importadas");
            System.Console.WriteLine("Iniciando importação dos professores...");

            var professores = controller.GetProfessores();
            var professorResult = controller.ImportarProfessores(professores);

            System.Console.WriteLine("Professores importados");
            System.Console.WriteLine("Iniciando importação das disciplinas dos professores...");
            
            var professorDisciplinaResult = controller.ImportarProfessorDisciplinas(professores);

            System.Console.WriteLine("Disciplinas dos professores importadas.");
            System.Console.WriteLine("Iniciando importação dos alunos...");

            var alunos = controller.GetAlunos();
            var alunosResult = controller.ImportarAlunos(alunos);
            
            System.Console.WriteLine("Alunos importados");
            System.Console.WriteLine("Iniciando importação das disciplinas dos alunos...");

            var alunoDisciplinasResult = controller.ImportarAlunoDisciplinas(alunos);

            System.Console.WriteLine("Disciplinas dos alunos importadas.");
            System.Console.WriteLine("Salvando os logs das importações realizadas...");

            var resultados = new Resultados()
            {
                CursosResult = cursosResult,
                DisciplinasResult = disciplinasResult,
                AlunosResult = alunosResult,
                ProfessoresResult = professorResult,
                DisciplinasAlunoResult = alunoDisciplinasResult,
                DisciplinasProfessorResult = professorDisciplinaResult
            };

            controller.SalvarLogs(resultados);            
        }

        static void SincronizarTest()
        { 
            var config = AppConfigurationFactory.Create();
            var builder = new AbstractServiceBuilder(config);

            builder.AddAuthenticationService(new AutenticacaoService())
                   .AddService(new SincroniaService())
                   .AddService(new ConsultaAcademicaService())
                   .Build();

            System.Console.WriteLine("ServiceBuilder object built");

            ImportadorServiceController controller = new ImportadorServiceController(builder, config);

            controller.ConfigureMoodleCache();

            System.Console.WriteLine("Moodle Cache built");

            var result = controller.Sincronizar();

            System.Console.WriteLine("Syncronizing process finished");

            controller.SalvarLogs(result, true);

            System.Console.WriteLine("Logs saved");
        }
        
        static void IniciarImportacao()
        {
            _TimerSincronia = new Timer()
            {
                AutoReset = true,
                Enabled = false
            };

            _TimerImportacao = new Timer()
            {
                AutoReset = true,
                Enabled = false
            };

            _TimerSincronia.Interval = _AppConfiguration.TempoSincronizacao;
            _TimerSincronia.Elapsed += new ElapsedEventHandler(TimerSincroniaElapsed);
            _TimerSincronia.Start();

            _TimerImportacao.Interval = _AppConfiguration.TempoImportacao;
            _TimerImportacao.Elapsed += new ElapsedEventHandler(TimerImportacaoElapsed);
            _TimerImportacao.Start();
        }


        static void Main(string[] args)
        {
            _AppConfiguration = AppConfigurationFactory.Create();
            _ServiceBuilder = new AbstractServiceBuilder(_AppConfiguration);

            ImportarTest();

            System.Console.ReadKey();
        }      


        protected static void TimerSincroniaElapsed(object sender, ElapsedEventArgs args)
        {
            if (!SincroniaIniciada)
            {
                Task.Run(() =>
                {
                    System.Console.WriteLine("Syncronizing Task started");

                    SincroniaIniciada = true;
                    SincronizarTest();
                    SincroniaIniciada = false;
                });                
            }
        }

        protected static void TimerImportacaoElapsed(object sender, ElapsedEventArgs args)
        {
            IAgendamentoExecucaoServicoRepository repository = new AgendamentoExecucaoServicoRepository();
            AgendamentoExecucaoServicoProxy proxy = new AgendamentoExecucaoServicoProxy(repository);
            IEnumerable<AgendamentoExecucaoServico> data = proxy.GetData();

            foreach (var item in data)
            {
                if (!item.PrecisaSerExecutado)
                    continue;

                if (item.ExecutaSincronia && !SincroniaIniciada)
                {
                    Task.Run(() =>
                    {
                        SincroniaIniciada = true;
                        SincronizarTest();
                        SincroniaIniciada = false;
                    });                    
                }

                if (item.ExecutaCompleto && !ImportacaoIniciada)
                {
                    Task.Run(() =>
                    {
                        ImportacaoIniciada = true;
                        ImportarTest();
                        ImportacaoIniciada = false;
                    });                    
                }
            }
        }
    }
}
