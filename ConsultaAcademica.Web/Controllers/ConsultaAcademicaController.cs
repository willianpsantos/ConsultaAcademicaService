using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service;
using ConsultaAcademica.Web.Models;
using Microsoft.Extensions.Configuration;
using ConsultaAcademica.Web;
using System.Net.Http;
using ConsultaAcademica.Service.Contract;

namespace ConsultaAcademica.Controllers
{
    public class ConsultaAcademicaController : Controller
    {
        private IConsultaAcademicaService _Service;        

        public ConsultaAcademicaController(IConsultaAcademicaService service)
        {
            _Service = service;
        }
        
        [HttpGet]
        public IActionResult Modalidades()
        {
            var data = _Service.GetModalidades();

            var pageable = new PageDataViewModel<Modalidade>()
            {
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult Modalidades(PageDataViewModel<Modalidade> model)
        {
            IEnumerable<Modalidade> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetModalidadeById(id);

                    if (entity != null)
                    {
                        data = new Modalidade[] { entity };
                    }
                    else
                    {
                        data = new Modalidade[] { };
                    }                    
                    break;

                case FilterType.ByName:
                    data = _Service.GetModalidades(model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult AreasConhecimento()
        {
            var data = _Service.GetAreasConhecimento();

            var pageable = new PageDataViewModel<AreaConhecimento>()
            {
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult AreasConhecimento(PageDataViewModel<AreaConhecimento> model)
        {
            IEnumerable<AreaConhecimento> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetAreaConhecimentoById(id);

                    if (entity != null)
                    {
                        data = new AreaConhecimento[] { entity };
                    }
                    else
                    {
                        data = new AreaConhecimento[] { };
                    }
                    break;

                case FilterType.ByName:
                    data = _Service.GetAreasConhecimento(model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult Cursos()
        {
            var data = _Service.GetCursosBySemestre(AppConfig.ApiCurrentSemester);

            var pageable = new PageDataViewModel<Curso>()
            {
                AditionalFilter = AppConfig.ApiCurrentSemester,
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult Cursos(PageDataViewModel<Curso> model)
        {
            IEnumerable<Curso> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetCursoById(id);

                    if (entity != null)
                    {
                        data = new Curso[] { entity };
                    }
                    else
                    {
                        data = new Curso[] { };
                    }
                    break;

                case FilterType.ByName:
                    data = _Service.GetCursosBySemestre(model.AditionalFilter, model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult Turmas()
        {
            var data = _Service.GetTurmasBySemestre(AppConfig.ApiCurrentSemester);

            var pageable = new PageDataViewModel<Turma>()
            {
                AditionalFilter = AppConfig.ApiCurrentSemester,
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult Turmas(PageDataViewModel<Turma> model)
        {
            IEnumerable<Turma> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetTurmaById(id);

                    if (entity != null)
                    {
                        data = new Turma[] { entity };
                    }
                    else
                    {
                        data = new Turma[] { };
                    }
                    break;

                case FilterType.ByName:
                    data = _Service.GetTurmasBySemestre(model.AditionalFilter, model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult Diretores()
        {
            var data = _Service.GetDiretores();

            var pageable = new PageDataViewModel<Diretor>()
            {
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult Diretores(PageDataViewModel<Diretor> model)
        {
            IEnumerable<Diretor> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetDiretorById(id);

                    if (entity != null)
                    {
                        data = new Diretor[] { entity };
                    }
                    else
                    {
                        data = new Diretor[] { };
                    }
                    break;

                case FilterType.ByName:
                    data = _Service.GetDiretores(model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult Coordenadores()
        {
            var data = _Service.GetCoordenadores();

            var pageable = new PageDataViewModel<Coordenador>()
            {
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult Coordenadores(PageDataViewModel<Coordenador> model)
        {
            IEnumerable<Coordenador> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetCoordenadorById(id);

                    if (entity != null)
                    {
                        data = new Coordenador[] { entity };
                    }
                    else
                    {
                        data = new Coordenador[] { };
                    }
                    break;

                case FilterType.ByName:
                    data = _Service.GetCoordenadores(model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult Alunos()
        {
            var data = _Service.GetAlunosBySemestre(AppConfig.ApiCurrentSemester);

            var pageable = new PageDataViewModel<Aluno>()
            {
                AditionalFilter = AppConfig.ApiCurrentSemester,
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult Alunos(PageDataViewModel<Aluno> model)
        {
            IEnumerable<Aluno> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetAlunoById(id);

                    if (entity != null)
                    {
                        data = new Aluno[] { entity };
                    }
                    else
                    {
                        data = new Aluno[] { };
                    }
                    break;

                case FilterType.ByName:
                    data = _Service.GetAlunosBySemestre(model.AditionalFilter, model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult Professores()
        {
            var data = _Service.GetProfessores();

            var pageable = new PageDataViewModel<Professor>()
            {
                Total = data.Count(),
                Data = data
            };

            return View(pageable);
        }

        [HttpPost]
        public IActionResult Professores(PageDataViewModel<Professor> model)
        {
            IEnumerable<Professor> data = null;

            switch (model.FilterType)
            {
                case FilterType.ById:
                    var id = Int64.Parse(model.FilteValue);
                    var entity = _Service.GetProfessorById(id);

                    if (entity != null)
                    {
                        data = new Professor[] { entity };
                    }
                    else
                    {
                        data = new Professor[] { };
                    }
                    break;

                case FilterType.ByName:
                    data = _Service.GetProfessores(model.FilteValue);
                    break;
            }

            model.Total = data.Count();
            model.Data = data;

            return View(model);
        }

        [HttpGet]
        public IActionResult DisciplinasAluno(long id)
        {
            var data = _Service.GetDisciplinasAluno(id);

            var pageable = new PageDataViewModel<Disciplina>()
            {
                Total = data.Count(),
                Data = data
            };

            return View("Disciplinas", pageable);
        }

        [HttpGet]
        public IActionResult DisciplinasProfessor(long id)
        {
            var data = _Service.GetDisciplinasProfessor(AppConfig.ApiCurrentSemester, id);

            var pageable = new PageDataViewModel<Disciplina>()
            {
                Total = data.Count(),
                Data = data
            };

            return View("Disciplinas", pageable);
        }
    }
}