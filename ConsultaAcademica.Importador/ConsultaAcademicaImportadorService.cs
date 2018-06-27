using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ConsultaAcademica.Core;
using ConsultaAcademica.Model;
using ConsultaAcademica.MoodleClient.Response;
using ConsultaAcademica.Proxy;
using ConsultaAcademica.Service;
using ConsultaAcademica.Service.Contract;
using ConsultaAcademica.Data.Interfaces;
using ConsultaAcademica.Data.Repositories;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Proxy.DataCache;
using System.IO;
using System.Reflection;

namespace ConsultaAcademica.Importador
{
    public partial class ConsultaAcademicaImportadorService : ServiceBase
    {
        private Timer _TimerSincronia;        
        private Timer _TimerImportacao;
        private AppConfiguration _AppConfiguration;
        private AbstractServiceBuilder _ServiceBuilder;
        private ImportadorServiceController _Controller;
                
        private bool _SincroniaIniciada = false;
        private bool _ImportacaoIniciada = false;


        public ConsultaAcademicaImportadorService()
        {
            InitializeComponent();

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
        }


        protected override void OnStart(string[] args)
        {
            this.RequestAdditionalTime(1800000);

            _AppConfiguration = AppConfigurationFactory.Create();
            _ServiceBuilder = new AbstractServiceBuilder(_AppConfiguration);

            _ServiceBuilder.AddConfiguration(_AppConfiguration)
                           .AddAuthenticationService(new AutenticacaoService())
                           .AddService(new SincroniaService())
                           .AddService(new ConsultaAcademicaService())
                           .Build();

            _Controller = new ImportadorServiceController(_ServiceBuilder, _AppConfiguration)
            {
                UseParallelism = false,
                CanLog = false
            };

            _Controller.ConfigureMoodleCache();

            _SincroniaIniciada = false;
            _ImportacaoIniciada = false;

            _TimerSincronia.Interval = _AppConfiguration.TempoSincronizacao;
            _TimerSincronia.Elapsed += new ElapsedEventHandler(TimerSincroniaElapsed);
            _TimerSincronia.Start();

            _TimerImportacao.Interval = _AppConfiguration.TempoImportacao;
            _TimerImportacao.Elapsed += new ElapsedEventHandler(TimerImportacaoElapsed);
            _TimerImportacao.Start();
        }

        protected override void OnStop()
        {            
            _TimerSincronia.Stop();
            _TimerSincronia.Dispose();

            _TimerImportacao.Stop();
            _TimerImportacao.Dispose();

            _ServiceBuilder.Dispose();
            _Controller.Dispose();
        }

        protected void Importar()
        {
            AppConfigurationFactory.Refresh(_AppConfiguration);

            _ServiceBuilder
                .AddConfiguration(_AppConfiguration)
                .Build();

            _Controller
                .AddServiceBuilder(_ServiceBuilder)
                .AddConfiguration(_AppConfiguration);

            _Controller.ConfigureMoodleCache();

            var professores = _Controller.GetProfessores();
            var alunos      = _Controller.GetAlunos();

            var cursosResult              = _Controller.ImportarCursos();
            var disciplinaResult          = _Controller.ImportarDisciplinas();
            var professorResult           = _Controller.ImportarProfessores(professores);
            var professorDisciplinaResult = _Controller.ImportarProfessorDisciplinas(professores);
            var alunosResult              = _Controller.ImportarAlunos(alunos);
            var alunoDisciplinasResult    = _Controller.ImportarAlunoDisciplinas(alunos);            

            var resultados = new Resultados()
            {
                CursosResult               = cursosResult,
                DisciplinasResult          = disciplinaResult,
                AlunosResult               = alunosResult,
                ProfessoresResult          = professorResult,
                DisciplinasAlunoResult     = alunoDisciplinasResult,
                DisciplinasProfessorResult = professorDisciplinaResult
            };

            _Controller.SalvarLogs(resultados);
        }

        protected void Sincronizar()
        {
            AppConfigurationFactory.Refresh(_AppConfiguration);

            _ServiceBuilder
                .AddConfiguration(_AppConfiguration)
                .Build();

            _Controller
                .AddServiceBuilder(_ServiceBuilder)
                .AddConfiguration(_AppConfiguration);

            var result = _Controller.Sincronizar();

            _Controller.SalvarLogs(result);
        }

        protected void TimerSincroniaElapsed(object sender, ElapsedEventArgs args)
        {
            if (!_SincroniaIniciada)
            {
                Task.Run(() => 
                {
                    _SincroniaIniciada = true;

                    Sincronizar();

                    _SincroniaIniciada = false;
                });                
            }
        }

        protected void TimerImportacaoElapsed(object sender, ElapsedEventArgs args)
        {
            IAgendamentoExecucaoServicoRepository repository = new AgendamentoExecucaoServicoRepository();
            AgendamentoExecucaoServicoProxy proxy = new AgendamentoExecucaoServicoProxy(repository);
            IEnumerable<AgendamentoExecucaoServico> data = proxy.GetData();

            foreach(var item in data)
            {
                if (!item.PrecisaSerExecutado)
                    continue;

                if (item.ExecutaSincronia && !_SincroniaIniciada)
                {
                    Task.Run(() =>
                    {
                        _SincroniaIniciada = true;

                        Sincronizar();

                        _SincroniaIniciada = false;
                    });
                    
                }

                if (item.ExecutaCompleto && !_ImportacaoIniciada)
                {
                    Task.Run(() =>
                    {
                        _ImportacaoIniciada = true;

                        Importar();

                        _ImportacaoIniciada = false;
                    });                    
                }
            }            
        }
    }
}
