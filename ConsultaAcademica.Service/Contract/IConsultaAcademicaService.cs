using System;
using System.Collections.Generic;
using ConsultaAcademica.Core;
using ConsultaAcademica.Model;

namespace ConsultaAcademica.Service.Contract
{
    public interface IConsultaAcademicaService : IAbstractService
    {
        Aluno GetAlunoById(long id);

        IEnumerable<Disciplina> GetDisciplinasAluno(long id);

        IEnumerable<Aluno> GetAlunos(string nomeMatriculaCpf = "");

        IEnumerable<Aluno> GetAlunosBySemestre(string periodoLetivo, string nomeMatriculaCpf = "");

        AreaConhecimento GetAreaConhecimentoById(long id);

        IEnumerable<AreaConhecimento> GetAreasConhecimento(string nome = "");

        Coordenador GetCoordenadorById(long id);

        IEnumerable<Coordenador> GetCoordenadores(string nomeMatriculaCpf = "", bool ativo = true);

        Curso GetCursoById(long id);

        IEnumerable<Curso> GetCursosBySemestre(string periodoLetivo, string nomeMatriculaCpf = "");

        Diretor GetDiretorById(long id);

        IEnumerable<Diretor> GetDiretores(string nomeMatriculaCpf = "", bool ativo = true);

        Modalidade GetModalidadeById(long id);

        IEnumerable<Modalidade> GetModalidades(string nome = "");

        Professor GetProfessorById(long id);

        IEnumerable<Disciplina> GetDisciplinasProfessor(string periodoLetivo, long id);

        IEnumerable<Professor> GetProfessores(string nomeMatriculaCpf = "");

        Turma GetTurmaById(long id);

        IEnumerable<Turma> GetTurmasBySemestre(string periodoLetivo, string nomeSigla = "", bool ativo = true);

        IEnumerable<Disciplina> GetDisciplinas(string periodoLetivo, string nomeDisciplina = "", string nomeCurso = "", string siglaCurso = "");
    }
}
