using ConsultaAcademica.Model;
using ConsultaAcademica.Service;
using ConsultaAcademica.Service.Contract;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Proxy
{
    public class SincroniaProxy
    {
        private ISincroniaService SincroniaService;
        private IConsultaAcademicaService ConsultaAcademicaService;

        public SincroniaProxy(ISincroniaService sincroniaService, IConsultaAcademicaService consultaAcademicaService)
        {
            SincroniaService = sincroniaService;
            ConsultaAcademicaService = consultaAcademicaService;
        }

        public IEnumerable<Aluno> GetData()
        {
            IEnumerable<AlunoCodigo> codigos = SincroniaService.GetAlunosParaSincronizar();

            if(codigos?.Count() == 0)
            {
                return new Aluno[] { };
            }

            List<Aluno> alunos = new List<Aluno>();

            foreach(var item in codigos)
            {
                var aluno = ConsultaAcademicaService.GetAlunoById(item.IdAluno);

                if(aluno?.IdAluno == 0)
                {
                    continue;
                }

                alunos.Add(aluno);
            }

            return alunos.ToArray();
        }
    }
}
