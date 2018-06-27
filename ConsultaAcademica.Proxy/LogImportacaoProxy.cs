using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ConsultaAcademica.Core;
using ConsultaAcademica.Data.Entities;
using ConsultaAcademica.Data.Interfaces;
using ConsultaAcademica.Data.Repositories;
using ConsultaAcademica.Model;
using ConsultaAcademica.MoodleClient.Response;
using System.Threading.Tasks;

namespace ConsultaAcademica.Proxy
{
    public class LogImportacaoProxy
    {
        private ILogImportacaoRepository<LogImportacao> Repository;

        public bool Sincronia { get; set; }


        public LogImportacaoProxy(ILogImportacaoRepository<LogImportacao> repository)
        {
            Repository = repository;
            Sincronia = false;
        }


        public void SaveCoursesLogs(SendResult<Curso, CategoryResponse> result)
        {
            var logs = new List<LogImportacao>();
            
            while(result.ImportedSuccessfully.Count > 0)
            {
                ImportedResult<Curso, CategoryResponse> item = result.ImportedSuccessfully.Dequeue();
                var log = LogImportacao.Parse<Curso, CategoryResponse>(item, TipoImportacao.Curso);
                log.Mensagem = $"Criação do curso [{item.Data.CursoDescricao}]";
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            while (result.NotImported.Count > 0)
            {
                NotImportedReason<Curso> item = result.NotImported.Dequeue();
                var log = LogImportacao.Parse<Curso>(item, TipoImportacao.Curso);
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            Repository.Save(logs);
        }

        public void SaveDisciplinesResult(ParallelSendResult<Disciplina, CourseResponse> result)
        {
            var logs = new List<LogImportacao>();

            while (!result.ImportedSuccessfully.IsEmpty)
            {
                ImportedResult<Disciplina, CourseResponse> item = null;
                bool success = result.ImportedSuccessfully.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = LogImportacao.Parse<Disciplina, CourseResponse>(item, TipoImportacao.Disciplina);
                log.Mensagem = $"Criação da disciplina [{item.Data.DisciplinaNome}] no curso [{item.Data.CursoDescricao}]";
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            while (!result.NotImported.IsEmpty)
            {
                NotImportedReason<Disciplina> item = null;
                bool success = result.NotImported.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = LogImportacao.Parse<Disciplina>(item, TipoImportacao.Disciplina);
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            Repository.Save(logs);
        }

        public void SaveAlumnLogs(ParallelSendResult<Aluno, UserResponse> result)
        {
            var logs = new List<LogImportacao>();

            while (!result.ImportedSuccessfully.IsEmpty)
            {
                ImportedResult<Aluno, UserResponse> item = null;
                bool success = result.ImportedSuccessfully.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = LogImportacao.Parse<Aluno, UserResponse>(item, TipoImportacao.Aluno);

                if (log.Suspenso)
                {
                    log.Mensagem = $"Suspensão do aluno [{item.Data.AlunoCpf}][{item.Data.AlunoNomeSocial}]. Situação acadêmica: [{item.Data.SituacaoAcademicaNome}]";
                }
                else
                {
                    log.Mensagem = $"Criação de usuário para o aluno [{item.Data.AlunoCpf}][{item.Data.AlunoNomeSocial}]";
                }

                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            while (!result.NotImported.IsEmpty)
            {
                NotImportedReason<Aluno> item = null;
                bool success = result.NotImported.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = LogImportacao.Parse<Aluno>(item, TipoImportacao.Aluno);
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            Repository.Save(logs);
        }

        public void SaveAlumnDisciplinesLogs(ParallelSendResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> result)
        {
            var logs = new List<LogImportacao>();

            while (!result.ImportedSuccessfully.IsEmpty)
            {
                ImportedResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse> item = null;
                bool success = result.ImportedSuccessfully.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = (item as AlunoDisciplinaImportedResult<AlunoDisciplinaViewModel, GetEnrolmentsByUserIdResponse>).Parse();
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            while (!result.NotImported.IsEmpty)
            {
                NotImportedReason<AlunoDisciplinaViewModel> item = null;
                bool success = result.NotImported.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = (item as AlunoDisciplinaNotImportedReason<AlunoDisciplinaViewModel>).Parse();
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            Repository.Save(logs);
        }

        public void SaveProfessorLogs(ParallelSendResult<Professor, UserResponse> result)
        {
            var logs = new List<LogImportacao>();

            while (!result.ImportedSuccessfully.IsEmpty)
            {
                ImportedResult<Professor, UserResponse> item = null;
                bool success = result.ImportedSuccessfully.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = LogImportacao.Parse<Professor, UserResponse>(item, TipoImportacao.Professor);
                log.Sincronia = Sincronia;

                if (log.Suspenso)
                {
                    log.Mensagem = $"Suspensão do professor [{item.Data.ProfessorCpf}][{item.Data.ProfessorNome}].";
                }
                else
                {
                    log.Mensagem = $"Criação de usuário para o professor [{item.Data.ProfessorCpf}][{item.Data.ProfessorNome}]";
                }

                logs.Add(log);
            }

            while (!result.NotImported.IsEmpty)
            {
                NotImportedReason<Professor> item = null;
                bool success = result.NotImported.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = LogImportacao.Parse<Professor>(item, TipoImportacao.Professor);
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            Repository.Save(logs);
        }

        public void SaveProfessorDisciplinesLogs(ParallelSendResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse> result)
        {
            var logs = new List<LogImportacao>();

            while (!result.ImportedSuccessfully.IsEmpty)
            {
                ImportedResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse> item = null;
                bool success = result.ImportedSuccessfully.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = (item as ProfessorDisciplinaImportedResult<ProfessorDisciplinaViewModel, GetEnrolmentsByUserIdResponse>).Parse();
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            while (!result.NotImported.IsEmpty)
            {
                NotImportedReason<ProfessorDisciplinaViewModel> item = null;
                bool success = result.NotImported.TryDequeue(out item);

                if (!success)
                {
                    continue;
                }

                var log = (item as ProfessorDisciplinaNotImportedReason<ProfessorDisciplinaViewModel>).Parse();
                log.Sincronia = Sincronia;
                logs.Add(log);
            }

            Repository.Save(logs);
        }
    }
}
