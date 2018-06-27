using ConsultaAcademica.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Proxy
{
    public class ProfessorDisciplinaViewModel
    {
        public Professor Professor { get; set; }

        public IEnumerable<Disciplina> Disciplinas { get; set; }
    }
}
