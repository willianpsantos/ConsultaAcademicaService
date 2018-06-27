using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultaAcademica.Model;
using ConsultaAcademica.Service;
using ConsultaAcademica.Service.Contract;
using ConsultaAcademica.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaAcademica.Web.Controllers
{
    public class SincroniaController : Controller
    {
        private ISincroniaService _Service;
        private IConsultaAcademicaService _ConsultaAcademicaService;

        public SincroniaController(ISincroniaService service, IConsultaAcademicaService consultaAcademicaService)
        {
            _Service = service;
            _ConsultaAcademicaService = consultaAcademicaService;
        }

        public IActionResult AlunosSincronia()
        {
            AlunoCodigo[] alunoIds = (AlunoCodigo[])_Service.GetAlunosParaSincronizar();
            var pageModel = new PageDataViewModel<Aluno>();

            if (alunoIds?.Length == 0)
            {
                pageModel.Total = 0;
                pageModel.Data = new Aluno[] { };
                return View(pageModel);
            }

            Queue<Aluno> queue = new Queue<Aluno>(alunoIds.Length);

            foreach(var item in alunoIds)
            {
                Aluno aluno = _ConsultaAcademicaService.GetAlunoById(item.IdAluno);
                queue.Enqueue(aluno);
            }

            pageModel.Total = queue.Count;
            pageModel.Data = queue.ToArray();

            return View(pageModel);
        }
    }
}